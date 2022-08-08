using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VigilantMeerkat.Db;
using VigilantMeerkat.Db.Model;
using VigilantMeerkat.Gateway.Auth;
using VigilantMeerkat.Gateway.Models;
using VigilantMeerkat.Micro.Base;

namespace VigilantMeerkat.Gateway.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AmqpService _amqp;
        private readonly AppDbContext _appDbContext;
        private readonly UserDbContext _userDbContext;
        private readonly JwtProvider _jwtProvider;

        public AccountController(/*AmqpService amqp,*/ UserDbContext userDbContext, JwtProvider jwtProvider, AppDbContext appDbContext)
        {
            //_amqp = amqp;
            _userDbContext = userDbContext;
            _jwtProvider = jwtProvider;
            _appDbContext = appDbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(CredentialsForm form)
        {
            var user = await _userDbContext.Users.FirstOrDefaultAsync(x => x.Email == form.Email);

            if (user != null)
            {
                return BadRequest();
            }

            var id = Guid.NewGuid();

            await _userDbContext.Users.AddAsync(new User
            {
                Id = id,
                Email = form.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(form.Password)
            });

            if (form.Email.EndsWith("admin.pl"))
            {
                await _appDbContext.AdminConfigs.AddAsync(new AdminConfig
                {
                    Id = id,
                    DefaultNotificationType = "EMAIL",
                    Email = form.Email,
                    PhoneNumber = "0"
                });

                await _appDbContext.SaveChangesAsync();
            }

            await _userDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(CredentialsForm form)
        {
            var user = await _userDbContext.Users.FirstOrDefaultAsync(x => x.Email == form.Email);

            if (user != null && BCrypt.Net.BCrypt.Verify(form.Password, user.Password))
            {
                return Ok(_jwtProvider.GenerateToken(new Account(user)));
            }

            return BadRequest();
        }

        [HttpGet("test")]
        public IActionResult TestAuth()
        {
            return Ok(new {
                status = true
            });
        }
    }
}
