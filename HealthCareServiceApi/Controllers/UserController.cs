using HealthCareServiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ModelsRepository;
using ModelsRepository.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace HealthCareServiceApi.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : CustomControllerBase
    {
        private readonly IConfiguration _config;
        private readonly JWTServices _JWTService;
        public UserController(IConfiguration config, IServiceUnit serviceunit) : base(serviceunit)
        {
            _config = config;
            _JWTService = new JWTServices(config, ServiceUnit);
        }

        [Route("Update")]
        [HttpPost]
        [Authorize]
        public IActionResult Update([FromForm] string User)
        {
            try
            {
                User _user = JsonSerializer.Deserialize<User>(User);

                User user = ServiceUnit.Users.GetUserBy(x => x.Id == CurrentUser.Id);

                user.Name = _user.Name;
                user.Email = _user.Email;
                user.Phone = _user.Phone;
                user.Lat = _user.Lat;
                user.Lng = _user.Lng;

                ServiceUnit.Users.SaveChanges();
                string token = _JWTService.GenerateToken(user);
                return Ok(new JsonResult(new { token, user }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetMember")]
        [HttpGet]
        [Authorize]
        public IActionResult GetMember([FromQuery] string id)
        {
            try
            {
                User user = ServiceUnit.Users.GetUserBy(x => x.Id == new Guid(id));
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("BlockUser")]
        [HttpPost]
        [Authorize]
        public IActionResult BlockUser([FromForm] string id)
        {
            try
            {
                User user = ServiceUnit.Users.GetUserBy(x => x.Id == new Guid(id));

                if (user.Role == "Block")
                {
                    user.Role = "User";
                }
                else
                {
                    // block service
                    List<Service> services = ServiceUnit.Service.GetAll(x => x.UserId == user.Id).ToList();
                    foreach (Service service in services)
                    {
                        service.IsActive = false;
                    }
                    ServiceUnit.Service.SaveChanges();

                    // block asked rquest
                    List<Request> requests = ServiceUnit.Request.GetAll(x => x.SenderId == user.Id).ToList();
                    foreach (Request request in requests)
                    {
                        request.status = 2;
                        ServiceUnit.Request.SaveChanges();
                        FailedRequest f_req = ServiceUnit.FailedRequest.GetUserBy(x => x.RequestId == request.Id);
                        if (f_req?.Id == 0)
                        {
                            FailedRequest failReqs = new FailedRequest()
                            {
                                RequestId = request.Id,
                                Date = new DateTime(),
                                Reason = "تم حظر طالب الخدمة"
                            };
                            ServiceUnit.FailedRequest.Add(failReqs);
                        }
                    }


                    // block accepted rquest
                    List<AcceptedRequest> AcceptedRequests = ServiceUnit.AcceptedRequest.GetAll(x => x.VolunteerId == CurrentUser.Id).ToList();
                    foreach (AcceptedRequest acr in AcceptedRequests)
                    {
                        Request request = ServiceUnit.Request.GetById(acr.RequestId);
                        request.status = 2;
                        ServiceUnit.Request.SaveChanges();

                        FailedRequest ff_req = ServiceUnit.FailedRequest.GetUserBy(x => x.RequestId == acr.RequestId);
                        if (ff_req?.Id == 0)
                        {

                            FailedRequest failReqs = new FailedRequest()
                            {
                                RequestId = request.Id,
                                Date = new DateTime(),
                                Reason = "تم حظر مقدم الخدمة"
                            };
                            ServiceUnit.FailedRequest.Add(failReqs);
                        }
                    }

                    user.Role = "Block";
                }

                ServiceUnit.Users.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetBlocedkUsers")]
        [HttpGet]
        [Authorize]
        public IActionResult GetBlocedkUsers()
        {
            try
            {

                List<User> users = ServiceUnit.Users.GetAll(x => x.Role == "Block").ToList();
                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }

        }

        [Route("SaveImage")]
        [HttpPost]
        [Authorize]
        public IActionResult SaveImage(List<IFormFile> battlePlans)
        {
            try
            {
                if (battlePlans != null && battlePlans.Count > 0)
                {
                    var path = _config["Directories:ServiceUserImage"];
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }


                    foreach (IFormFile file in battlePlans)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = CurrentUser.Id.ToString(); // Path.GetFileName(file.FileName);
                            //var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            //var fileExtension = Path.GetExtension(fileName);
                            var newFileName = String.Concat(fileName, ".jpg");

                            using (FileStream fs = System.IO.File.Create(String.Concat(path, newFileName)))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                        }
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
    }
}
