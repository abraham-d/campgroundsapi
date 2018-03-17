using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using campgrounds_api.Data;
using campgrounds_api.DTOs;
using campgrounds_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace campgrounds_api.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository repo;
        private readonly IConfiguration config;
        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            this.config = config;
            this.repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]UserForRegisterDto user)
        {
            if (!string.IsNullOrWhiteSpace(user.UserName))
                user.UserName = user.UserName.ToLower();

            if (await repo.UserExists(user.UserName))
                ModelState.AddModelError("UserName", "UserName is already taken.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToCreate = new User()
            {
                UserName = user.UserName,
                Created = DateTime.Now,
                LastActive = DateTime.Now
            };

            var createUser = await repo.Register(userToCreate, user.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserForLoginDto user)
        {
            if (!string.IsNullOrWhiteSpace(user.UserName))
                user.UserName = user.UserName.ToLower();

            var userFromRepo = await repo.Login(user.UserName, user.Password);
            if (userFromRepo == null)
                return Unauthorized();

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokendescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                            new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                            new Claim(ClaimTypes.Name, userFromRepo.UserName) }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(
                                             new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                                                 config.GetSection("AppSettings:Token").Value)),
                                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokendescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { tokenString });
        }
    }
}