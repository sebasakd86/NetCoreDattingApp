using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DattingApp.API.Data;
using DattingApp.API.DTO;
using DattingApp.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace DattingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AuthController(
            IConfiguration config, 
            IMapper mapper,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            this._config = config;
            this._mapper = mapper;
            this._userManager = userManager;
            this._signInManager = signInManager;
        }

        //Parameters are all infered via ApiController. Otherwise FromBody within params.
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDTO usrDto)
        {
            User user = _mapper.Map<User>(usrDto);

            var result = await _userManager.CreateAsync(user, usrDto.Password);            

            if(result.Succeeded)
            {
                var usrToReturn = _mapper.Map<UserForDetailedDTO>(user);

                return CreatedAtRoute("GetUser", new { controller = "Users", id = user.Id }, usrToReturn);
            }
            return BadRequest(result.Errors);            
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDTO usrDto)
        {
            var user = await _userManager.FindByNameAsync(usrDto.UserName);            

            var res = await _signInManager.CheckPasswordSignInAsync(
                user, 
                usrDto.Password,
                false
            );
            if(res.Succeeded)
            {
                var appUsr = _mapper.Map<UserForListDTO>(user);
                return Ok(new
                {
                    token = GenerateJWTToken(user),
                    user = appUsr //to avoid cascading changes in our spa
                });
            }
            return Unauthorized();
        }
        private string GenerateJWTToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            //Sign the token
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value)
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            //Describe the token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

    }
}