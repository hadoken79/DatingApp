using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService) : base(context) 
        {
            _tokenService = tokenService;
        }
        

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)//cause of the attribute [ApiController in base class, all params from body are avaiable]
        {
            if (await UserExists(registerDto.Username)) return BadRequest("Username is taken!");//Badrequest is part of ActionResult

            using var hmac = new HMACSHA512();

            using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(registerDto.Password));//for handeling data in Objects, as they are send via api, there are DTO's used

            var user = new AppUser
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = await hmac.ComputeHashAsync((Stream)stream),//if used sync method, Encoding.UTF8.GetBytes(password) could be passed directly
                PasswordSalt = hmac.Key
            };
            _context.Add(user); //tells ef to track this (like staging)
            await _context.SaveChangesAsync(); // actual saving in db
            return new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto logindto)
        {

            var user = await _context.Users.SingleOrDefaultAsync<AppUser>(x => x.UserName == logindto.Username.ToLower());

            if (user == null) return Unauthorized("Username or Password invalid");

            using var hmac = new HMACSHA512(user.PasswordSalt);
            using MemoryStream passwordStream = new MemoryStream(Encoding.UTF8.GetBytes(logindto.Password));
            var computedHash = await hmac.ComputeHashAsync((Stream)passwordStream);

            if (user.PasswordHash.SequenceEqual(computedHash))
            {
                return new UserDto()
                {
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user)
                };
            }
            else
            {
                return Unauthorized("Username or Password invalid");
            }
        }

        private async Task<bool> UserExists(string username) => await _context.Users.AnyAsync(x => x.UserName == username.ToLower());

    }
}