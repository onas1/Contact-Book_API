using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskContactBook.Auth;

namespace TaskContactBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _user;
        private readonly IConfiguration config;

        public AuthController(UserManager<AppUser> user, IConfiguration configuration )
        {
            _user = user;
            config = configuration;
        }

        [HttpPost("Login")]

        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            var user = await _user.FindByEmailAsync(model.Email);
            if(user == null)
            {
                return BadRequest("User not found");
            }
            var password = await _user.CheckPasswordAsync(user, model.Password);
            if (!password)
            {
                return BadRequest("Invalid credentials");
            }

            var roles = await _user.GetRolesAsync(user);
            var UserRoles = roles.ToArray() ;


            var token = AuthService.GenerateToken(user.Email, user.Id, model.Email, config, UserRoles);
            return Ok(token);
            
        }
    }
}
