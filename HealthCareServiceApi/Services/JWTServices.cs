using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ModelsRepository;
using ModelsRepository.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HealthCareServiceApi.Services
{
    public class JWTServices : BaseServices
    {
        private readonly IConfiguration _config;
        public JWTServices(IConfiguration config, IServiceUnit serviceunit) : base(serviceunit)
        {
            _config = config;
        }

        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // user information encoding  in jwt topken
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Audience"],
              claims,
              expires: DateTime.Now.AddMinutes(1000 * 200),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public User ChechAuthenticate(User userLogin)
        {
            var currentUser =
                _serviceunit.Users.CheckUserLogin(o =>
                (o.Email.ToLower() == userLogin.Email.ToLower() || o.Phone == userLogin.Phone)
            && o.Password == userLogin.Password);

            if (currentUser != null)
            {
                return currentUser;
            }

            return null;
        }

    }
}
