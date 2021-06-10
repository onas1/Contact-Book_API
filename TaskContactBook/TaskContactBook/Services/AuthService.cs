using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace TaskContactBook.Auth
{
    public static class AuthService
    {
        public static object GenerateToken(string UserName, string UserId, string Email, IConfiguration Config, string[] roles)
        {
            //3. Create and Set Up Claims
            var Claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, UserName),
                new Claim(ClaimTypes.Email, Email),
                new Claim(ClaimTypes.NameIdentifier, UserId)
            };

            //Create and setup roles
            foreach ( var role in roles)
            {
                Claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //2. create security token descriptor
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(Claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Config.GetSection("JWT:JWTSigninKey").Value)),
                SecurityAlgorithms.HmacSha256Signature)


            };

            //1. Create a token Handler
            var TokenHandler = new JwtSecurityTokenHandler();
            var TokenCreated = TokenHandler.CreateToken(securityTokenDescriptor);
            return new 
            {
                Token = TokenHandler.WriteToken(TokenCreated),
                id = UserId
            };
        }
    }
}
