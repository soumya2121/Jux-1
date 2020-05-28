using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppDal.Context;
using DatingAppDal.Model;
using DatingAppDal.Repositories.DatingRepo;
using DatingAppWebApi.Dto;
using DatingAppWebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace DatingAppWebApi.Controllers
{ 
    
    [Authorize]
    [Route("api/Users")]
    [ServiceFilter(typeof(LogUserActivity))]
    public class UserController:Controller
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UserController(IDatingRepository repo, IMapper mapper)
        {
            _repo= repo;
            _mapper=mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers(UserParams userParams)
        {
            var CurrentUserId= int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var CurrentUser= await _repo.GetUser(CurrentUserId);
            userParams.UserId= CurrentUserId;
            if(string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = CurrentUser.Gender =="male" ?"female": "male";
            }
            var users= await _repo.GetUsers(userParams);
            var UserLists= _mapper.Map<IEnumerable<UserListDto>>(users);
            Response.AddPaginationHeader(users.CurrentPage,users.PageSize,users.TotalPage,users.TotalCount);
            return Ok(UserLists);
        }
        
        [HttpGet("{id}",Name="GetUser")]
        public async Task<IActionResult> GetUser(int Id)
        {
            var user= await _repo.GetUser(Id);
            var UserDetails= _mapper.Map<UserDetailsDto>(user);
            return Ok (UserDetails);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id,[FromBody] UserEditDto userEditDto)
        {
            if(!ModelState.IsValid)
            return BadRequest(ModelState);

            var LoggedInUser= Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var UserFromDb= await _repo.GetUser(id);
            if(UserFromDb==null)
           
            throw new Exception($"there is no such user with id: {id}");
            if(LoggedInUser!= UserFromDb.Id)
            return Unauthorized();
            _mapper.Map(userEditDto,UserFromDb);
           await _repo.SaveAll();
           return NoContent();

           throw new Exception($"updating user {id} failed");
        }

        [HttpPost("{userId}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int userId,int recipientId)
        {
            int isValidUser= Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if(userId!=isValidUser)
            return Unauthorized();
            var likee= await _repo.GetUser(recipientId);
            if(likee==null)
            return NotFound("You can't like an user that does not exist");
            var like= await _repo.GetLike(userId,recipientId);
            if(like!=null)
            return BadRequest("You already like " + likee.KnownAs);
            like= new Like{
              LikerId= userId,
              LikeeId= recipientId
            };
            _repo.Add<Like>(like);
            if(!await _repo.SaveAll())
            return BadRequest("Liking "+likee.KnownAs+" failed");

            return Ok();
        }

        
    }
}