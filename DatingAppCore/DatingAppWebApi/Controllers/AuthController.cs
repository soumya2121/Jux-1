using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingAppDal.Context;
using DatingAppDal.Model;
using DatingAppDal.Repositories.AuthRepo;
using DatingAppWebApi.Dto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingAppWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private IAuthRepository _repo;
        private IConfiguration _config;
        private IMapper _mapper;
        
        public AuthController(IAuthRepository repository,IConfiguration configuration, IMapper mapper)
        {
            _repo = repository;
            _config = configuration;
            _mapper= mapper;
        }
       
       [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody]UserRegistrationDto userRegistrationDto)
        {
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
          
            if(! await _repo.UserExist(userRegistrationDto.Username.ToLower()))
            {
                ModelState.AddModelError("Username", "Username already taken try someother username");
                return BadRequest(ModelState);
            }

            var User = _mapper.Map<User>(userRegistrationDto);
            var CreatedUser= await _repo.RegisterUser(User,userRegistrationDto.Password); 
            var ReturnUser= _mapper.Map<UserDetailsDto>(User);
            return CreatedAtRoute("GetUser",new {Controller="user", id=CreatedUser.Id},ReturnUser);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserRegistrationDto userRegistrationDto)
        {
            var user = await _repo.LoginUser(userRegistrationDto.Username,userRegistrationDto.Password);
            if(user==null)
            {
                return Unauthorized();
            }
            // generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var userFromDb = _mapper.Map<UserListDto>(user);
            return Ok(new {tokenString, userFromDb});
        }
    }
}
