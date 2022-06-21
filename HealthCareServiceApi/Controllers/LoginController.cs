using ModelsRepository.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ModelsRepository;
using HealthCareServiceApi.Services;
using Microsoft.AspNetCore.Cors;
using System.Text.Json;

namespace HealthCareServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : CustomControllerBase
    {
        private readonly JWTServices _JWTService;
        public LoginController(IConfiguration config, IServiceUnit serviceunit) : base(serviceunit)
        {
            _JWTService = new JWTServices(config, ServiceUnit);
        }


        [HttpPost]
        [Route("Login")]
        [AllowAnonymous]
        [EnableCors("_myAllowSpecificOrigins")]
        public IActionResult Login([FromForm] string Email, [FromForm] string Password)
        {
            try
            {
                User user = _JWTService.ChechAuthenticate(new User() { Email = Email, Phone = Email, Password = Password });

                if (user != null)
                {
                    string token = _JWTService.GenerateToken(user);
                    User _user = ServiceUnit.Users.GetUserBy(x => x.Email == user.Email);
                    if(user.Role == "Block")
                    {
                        return Ok("تم حظر المستخدم");
                    }
                    return Ok(new JsonResult(new { token, user }));
                }
            }
            catch (Exception e)
            {
                return NotFound("User not found");
            }

            return NotFound("User not found");
        }


        [HttpGet]
        [Route("UpdateLogin")]
        [Authorize]
        public IActionResult UpdateLogin()
        {
            try
            {
                User user = CurrentUser;

                if (user != null)
                {
                    string token = _JWTService.GenerateToken(user);
                    User _user = ServiceUnit.Users.GetUserBy(x => x.Email == user.Email);
                    return Ok(new JsonResult(new { token, user }));
                }
            }
            catch (Exception e)
            {
                return NotFound("User not found");
            }

            return NotFound("User not found");
        }


        [HttpPost]
        [Route("Register")]
        [AllowAnonymous]
        [EnableCors("_myAllowSpecificOrigins")]
        public IActionResult Register([FromForm] string strUser, [FromForm] string ServicesInfo)
        {
            try
            {
                User user = JsonSerializer.Deserialize<User>(strUser);
                if (user != null)
                {
                    user.Role = "User";
                    User _user = ServiceUnit.Users.Add(user);

                    if (_user != null)
                    {
                        var token = _JWTService.GenerateToken(user);
                        return Ok(new JsonResult(new { token, user = _user }));
                    }
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }

            return NotFound("User not found");
        }
    }
}
