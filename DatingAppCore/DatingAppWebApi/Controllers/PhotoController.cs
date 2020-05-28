using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingAppDal.Model;
using DatingAppDal.Repositories.DatingRepo;
using DatingAppWebApi.Dto;
using DatingAppWebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingAppWebApi.Controllers
{
    [Authorize]
    [Route("api/users/{UserId}/photos")]
    public class PhotoController:Controller
    {
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private readonly IDatingRepository _repo;
        private readonly Cloudinary _cloudinary;
        private readonly IMapper _mapper;
        public PhotoController(IOptions<CloudinarySettings> CloudinaryConfig, IDatingRepository repo, IMapper mapper)
        {
            _cloudinaryConfig=CloudinaryConfig;
            _repo=repo;
            _mapper=mapper;

            Account acc= new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );
            _cloudinary= new Cloudinary(acc);
        }

         [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoToReturnDto>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> UploadUserPhoto(PhotoCreationDto photoDto, int UserId)
        {
            var userFromDb = await _repo.GetUser(UserId);
            if(userFromDb==null)
            return BadRequest();
            var userToCheck= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if(userFromDb.Id != userToCheck)
            return Unauthorized();
            var file= photoDto.File;
            var uploadResult= new ImageUploadResult();
            if(file.Length>0)
            {
              using(var stream= file.OpenReadStream())
              {
                 var uploadParams= new ImageUploadParams()
                 {
                    File = new FileDescription(file.FileName, stream),
                    Transformation= new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                 };

                 uploadResult = _cloudinary.Upload(uploadParams);
              }
            }
           photoDto.Url= uploadResult.Uri.ToString();
           photoDto.PublicId= uploadResult.PublicId;
           var photo= _mapper.Map<Photo>(photoDto);
           photo.User= userFromDb;
           if(!userFromDb.Photos.Any(x=> x.IsMain))
           {
               photo.IsMain=true;
           }
           userFromDb.Photos.Add(photo);
           if(await _repo.SaveAll())
           {
              var photoToReturn= _mapper.Map<PhotoToReturnDto>(photo);
              return CreatedAtRoute("GetPhoto",new { id = photo.Id },photoToReturn);
           }
          return BadRequest("Could not add the photo");
        }

        [HttpPost("{photoid}/setMain")]
        public async Task<IActionResult> setMainPhoto(int userId, int photoId)
        {
            var user= await _repo.GetUser(userId);
            if(user == null)
            return NotFound("User not found with this id");
            if(user.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            Photo photo = await _repo.GetPhoto(photoId);
            if(photo ==null)
            return NotFound();
            if(photo.IsMain== true)
            return BadRequest("Already a main photo");
            Photo userMainPhoto= await _repo.GetUserMainPhoto(userId);
            if(userMainPhoto!=null)
            {
                userMainPhoto.IsMain=false;
                photo.IsMain=true;
               if( await _repo.SaveAll())
                return NoContent();
            }
            return BadRequest("Main photo could not be saved");
            
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int UserId, int id)
        {
            var userFromDb= await _repo.GetUser(UserId);
            if(userFromDb==null || userFromDb.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
            var photoFromDb= await _repo.GetPhoto(id);
            if(photoFromDb==null)
            return BadRequest("No photo found");
            if(photoFromDb.IsMain)
            return BadRequest("Cannot delete main photo");
            if(photoFromDb.PublicId!=null && photoFromDb.PublicId!="")
            {
                var deleteParams= new DeletionParams(photoFromDb.PublicId);
                var result= _cloudinary.Destroy(deleteParams);
                if(result.Result=="ok")
                {
                  _repo.Delete(photoFromDb);
                }
            }
            else if(photoFromDb.PublicId== null || photoFromDb.PublicId=="")
            {
                _repo.Delete(photoFromDb);
            }
           if(await _repo.SaveAll())
           {
               return Ok();
           }
           return BadRequest ("Photo deletion failed");
        }
    }
}