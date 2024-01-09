using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Data.Repositories;
using API.Dtos;
using API.Entity;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SQLitePCL;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserRepository _context;
        private readonly TokenService _tokenService;
        public AccountController(IUserRepository context, TokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;

        }

        [HttpPost]
        public async Task<ActionResult<User>> Register(RegisterDto user)
        {

            if (await UserExist(user.UserName))
            {
                return BadRequest("User Already Exist");
            }

            using var hmac = new HMACSHA512();

            var userToRegister = new User
            {

                UserName = user.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(user.Password)),
                PasswordSalt = hmac.Key
            };

            await _context.CreateUser(userToRegister);
            await _context.SaveAll();

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(userToRegister)

            });

        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto Login)
        {

            var user = await _context.GetUserByNameAsync(Login.UserName.ToLower());
            if (user == null) return Unauthorized("User Name note valid");
            byte[] ComputedHash;

            using (var hmac = new HMACSHA512(user.PasswordSalt))
            {
                ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(Login.Password));

            }
            for (int i = 0; i < ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("User pasddword incorrect");
                }

            }

            return Ok(new UserDto
            {
                UserName = user.UserName,
                Token = _tokenService.CreateToken(user)

            });
        }

        private async Task<bool> UserExist(string UserName)
        {

            return await _context.IsUserExist(UserName);
        }


    }



}
