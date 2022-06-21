using HealthCareServiceApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using ModelsRepository;
using ModelsRepository.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace HealthCareServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : CustomControllerBase
    {
        private readonly IConfiguration _config;
        public ServiceController(IConfiguration config, IServiceUnit serviceunit) : base(serviceunit)
        {
            _config = config;
        }

        [Route("SaveServiceType")]
        [HttpPost]
        public IActionResult SaveServiceType(ServiceType _serviceType)
        {
            try
            {
                ServiceType ServiceType = ServiceUnit.ServiceType.Add(_serviceType);
                return Ok(ServiceType);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveService")]
        [HttpPost]
        [Authorize]
        public IActionResult SaveService([FromForm] string service, List<IFormFile> battlePlans, [FromForm] string Id)
        {
            try
            {
                User user = ServiceUnit.Users.GetUserBy(x => x.Id == CurrentUser.Id);
                Service _service = JsonSerializer.Deserialize<Service>(service);
                ServiceType type = ServiceUnit.ServiceType.GetUserBy(x => x.Id == _service.TypeId);
                Service Service;
                if (string.IsNullOrEmpty(Id) || Id == "null")
                {
                    _service.UserId = CurrentUser.Id;
                    _service.IsActive = !type.Category.Equals("3");
                    Service = ServiceUnit.Service.Add(_service);
                }
                else
                {
                    Service = ServiceUnit.Service.GetById(Convert.ToInt32(Id));
                    Service.AgeFrom = _service.AgeFrom;
                    Service.AgeTo = _service.AgeTo;
                    Service.TypeId = _service.TypeId;
                    Service.Gender = _service.Gender;
                    Service.Attachments = _service.Attachments;
                    ServiceUnit.Service.SaveChanges();
                }

                if (user.Role != "Admin")
                {
                    user.Role = "Volunteer";
                    ServiceUnit.Users.SaveChanges();
                }

                if (battlePlans != null && battlePlans.Count > 0)
                {
                    var path = String.Concat(_config["Directories:ServiceAttachment"], Service.Id, "/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    foreach (IFormFile file in battlePlans)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            //var myUniqueFileName = Convert.ToString(Guid.NewGuid());
                            //var fileExtension = Path.GetExtension(fileName);
                            //var newFileName = String.Concat(myUniqueFileName, fileExtension);

                            using (FileStream fs = System.IO.File.Create(String.Concat(path, fileName)))
                            {
                                file.CopyTo(fs);
                                fs.Flush();
                            }

                            ServiceUnit.ServiceAttachment.Add(new ServiceAttachment()
                            { Attachment = fileName.ToString(), ServiceId = Service.Id });
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

        [Route("GetUnApprovalService")]
        [HttpGet]
        [Authorize]
        public IActionResult GetUnApprovalService()
        {
            try
            {
                List<Service> services = ServiceUnit.Service.GetAll(x => x.IsActive == false).ToList();
                List<ServiceAttachment> serviceAttachment = ServiceUnit.ServiceAttachment.GetAll(x => x.Id != null).ToList();
                List<ServiceType> types = ServiceUnit.ServiceType.GetAll(x => x.Id != null).ToList();
                List<User> users = ServiceUnit.Users.GetAll(x => x.Id != null).ToList();

                return Ok(services);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("ApproveService")]
        [HttpPost]
        [Authorize]
        public IActionResult ApproveService([FromForm] int id)
        {
            try
            {
                Service service = ServiceUnit.Service.GetById(id);
                service.IsActive = true;
                ServiceUnit.Service.SaveChanges();
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("DeleteService")]
        [HttpPost]
        [Authorize]
        public IActionResult DeleteService([FromForm] int id)
        {
            try
            {
                Service service = ServiceUnit.Service.GetById(id);
                List<Service> services = ServiceUnit.Service.GetAll(x => x.UserId == service.UserId).ToList();
                User user = ServiceUnit.Users.GetUserBy(x => x.Id == service.UserId);

                if (services.Count == 1 && !user.Role.Equals("Admin"))
                {
                    user.Role = "User";
                    ServiceUnit.Users.SaveChanges();
                }

                ServiceUnit.Service.RemoveObj(service);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("RemoveService")]
        [HttpPost]
        [Authorize]
        public IActionResult RemoveService([FromForm] int Id)
        {
            try
            {
                ServiceUnit.Service.RemoveObj(ServiceUnit.Service.GetById(Id));
                User user = ServiceUnit.Users.GetUserBy(x => x.Id == CurrentUser.Id);
                List<Service> services = ServiceUnit.Service.GetAll(x => x.UserId == user.Id).ToList();

                if (services.Count == 0 && !user.Role.Equals("Admin"))
                {
                    user.Role = "User";
                    ServiceUnit.Users.SaveChanges();
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [NonAction]
        public double GetUserRate(Guid id)
        {
            List<UserRating> rates = ServiceUnit.UserRating.GetAll(x => x.request.SenderId == id /*&& !x.IsVolunteer*/).ToList();
            if (rates.Count > 5)
            {
                return rates.Sum(x => x.Value) / rates.Count;
            }
            else
            {
                return 2.0;
            }
        }

        [Route("SaveRequest")]
        [HttpPost]
        [Authorize]
        public IActionResult SaveRequest([FromForm] string request, [FromForm] bool CurrentLocation, [FromForm] bool CurrentInfo)
        {
            try
            {
                Request _r = JsonSerializer.Deserialize<Request>(request);
                Request _request = new Request()
                {
                    Date = DateTime.Now,
                    SenderId = CurrentUser.Id,
                    Description = _r.Description,
                    Lattiud = !CurrentLocation ? _r.Lattiud : CurrentUser.Lat,
                    Longtiud = !CurrentLocation ? _r.Longtiud : CurrentUser.Lng,
                    ExpireTime = _r.ExpireTime,
                    SeviceTypeId = _r.SeviceTypeId,
                    PGender = !CurrentInfo ? _r.PGender : CurrentUser.Gender,
                    PDescription = _r.PDescription,
                    PAge = !CurrentInfo ? _r.PAge : (DateTime.Today.Year - CurrentUser.BirthDate.Year),
                    PName = !CurrentInfo ? _r.PName : CurrentUser.Name,
                    VGender = _r.VGender,
                    status = 0
                };
                Request Request = ServiceUnit.Request.Add(_request);
                return Ok(Request);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveDeliveredRequest")]
        [HttpPost]
        public IActionResult SaveDeliveredRequest(DeliveredRequest _deliveredRequest)
        {
            try
            {
                DeliveredRequest DeliveredRequest = ServiceUnit.DeliveredRequest.Add(_deliveredRequest);
                return Ok(DeliveredRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveAcceptedRequest")]
        [HttpPost]
        public IActionResult SaveAcceptedRequest(AcceptedRequest _acceptedRequest)
        {
            try
            {
                Request _req = ServiceUnit.Request.GetUserBy(x => x.Id == _acceptedRequest.RequestId);
                Service _service = ServiceUnit.Service.GetUserBy(x => x.UserId == _acceptedRequest.VolunteerId && _req.SeviceTypeId == x.TypeId);
                _req.ServiceId = _service.Id;
                ServiceUnit.Request.SaveChanges();
                AcceptedRequest AcceptedRequest = ServiceUnit.AcceptedRequest.Add(_acceptedRequest);
                return Ok(AcceptedRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveFailedRequest")]
        [HttpPost]
        public IActionResult SaveFailedRequest(FailedRequest _failedRequest)
        {
            try
            {
                FailedRequest FailedRequest = ServiceUnit.FailedRequest.Add(_failedRequest);

                return Ok(FailedRequest);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveUserRating")]
        [HttpPost]
        public IActionResult SaveUserRating(UserRating _userRating)
        {
            try
            {
                UserRating UserRating = ServiceUnit.UserRating.Add(_userRating);
                return Ok(UserRating);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveReport")]
        [HttpPost]
        public IActionResult SaveReport(Report _report)
        {
            try
            {
                Report Report = ServiceUnit.Report.Add(_report);
                return Ok(Report);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetVolunteerRequest")]
        [Authorize]
        [HttpGet]
        public IActionResult GetVolunteerRequest()
        {
            try
            {
                User user = CurrentUser;
                List<Request> RequestsInScope = ServiceUnit.Request.GetAll(x => x.status == 0 && !x.seviceType.Category.Equals("2")).ToList();
                List<Service> UserServices = ServiceUnit.Service.GetAll(x => x.UserId == user.Id && x.IsActive == true).ToList();
                if (UserServices.Count == 0)
                {
                    return BadRequest(new JsonResult(new { UserServices, RequestsInScope }));
                }
                List<Request> InScopeRequests = RequestsInScope.FindAll(x =>
                UserServices.FirstOrDefault(e => e.TypeId == x.SeviceTypeId && x.SenderId != e.UserId && ((e.Gender == x.PGender || e.Gender == 3) && (user.Gender == x.VGender || x.VGender == 3)) && ((x.PAge <= e.AgeTo && x.PAge >= e.AgeFrom) || e.AgeFrom == -1)) != null &&
                (1 >= CalculateDistance(x.Lattiud, x.Longtiud, user.Lat, user.Lng)));

                List<Request> AroundScopeRequests = RequestsInScope.FindAll(x =>
               UserServices.FirstOrDefault(e => e.TypeId == x.SeviceTypeId && ((e.Gender == x.PGender || e.Gender == 3) && (user.Gender == x.VGender || x.VGender == 3)) && x.SenderId != e.UserId && ((x.PAge <= e.AgeTo && x.PAge >= e.AgeFrom) || e.AgeFrom == -1)) != null &&
               (1 < CalculateDistance(x.Lattiud, x.Longtiud, user.Lat, user.Lng) && 3 >= CalculateDistance(x.Lattiud, x.Longtiud, user.Lat, user.Lng)));

                foreach (Request request in InScopeRequests)
                {
                    request.seviceType = ServiceUnit.ServiceType.GetById(request.SeviceTypeId);
                    int count = ServiceUnit.RequestReceivers.Count(x => x.RequestId == request.Id && x.UserId == user.Id);
                    if (count == 0)
                    {
                        ServiceUnit.RequestReceivers.Add(new RequestReceivers()
                        {
                            RequestId = request.Id,
                            UserId = user.Id,
                            distance = CalculateDistance(request.Lattiud, request.Longtiud, user.Lat, user.Lng)
                        });
                    }
                }

                foreach (Request request in AroundScopeRequests)
                {
                    request.seviceType = ServiceUnit.ServiceType.GetById(request.SeviceTypeId);
                    int count = ServiceUnit.RequestReceivers.Count(x => x.RequestId == request.Id && x.UserId == user.Id);
                    if (count == 0)
                    {
                        ServiceUnit.RequestReceivers.Add(new RequestReceivers()
                        {
                            RequestId = request.Id,
                            UserId = user.Id,
                            distance = CalculateDistance(request.Lattiud, request.Longtiud, user.Lat, user.Lng)
                        });
                    }
                }
                return Ok(new JsonResult(new { InScopeRequests, AroundScopeRequests }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
        [Route("GetAttachmentsService")]
        [Authorize]
        [HttpGet]
        public IActionResult GetAttachmentsService()
        {
            try
            {
                User user = CurrentUser;
                List<ServiceType> ServicesTypes = ServiceUnit.ServiceType.GetAll(x => x.Category == "2").ToList();
                List<Service> ServicesInScope = ServiceUnit.Service.GetAll(x => x.UserId != CurrentUser.Id && x.IsActive == true).ToList().FindAll(x => ServicesTypes.Exists(y => y.Id == x.TypeId));

                List<ServiceAttachment> ServiceAttachments = ServiceUnit.ServiceAttachment.GetAll(x => x.Id != -1).ToList();

                List<Service> InScopeServices = ServicesInScope; // ServicesInScope.FindAll(x => 1000 >= CalculateDistance(x.user.Lat, x.user.Lng, user.Lat, user.Lng));

                List<Service> AroundScopeServices = ServicesInScope; //  ServicesInScope.FindAll(x =>
                                                                     // (1000 < CalculateDistance(x.user.Lat, x.user.Lng, user.Lat, user.Lng)) &&
                                                                     //        (3000 >= CalculateDistance(x.user.Lat, x.user.Lng, user.Lat, user.Lng)));

                List<User> users = ServiceUnit.Users.GetAll(x => x.Id != null).ToList();
                List<Request> requests = ServiceUnit.Request.GetAll(x => x.SenderId == user.Id).ToList();
                requests = requests.Where(x => ServicesInScope.FirstOrDefault(z => x.ServiceId == z.Id) != null).ToList();
                return Ok(new JsonResult(new { InScopeServices, AroundScopeServices, requests }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetProvidedList")]
        [Authorize]
        [HttpGet]
        public IActionResult GetProvidedList()
        {
            try
            {
                User user = CurrentUser;
                List<Service> Services = ServiceUnit.Service.GetAll(x => x.UserId == user.Id).ToList();
                List<ServiceType> ServiceTypes = ServiceUnit.ServiceType.GetAll(x => x.Id != -1).ToList();

                return Ok(new JsonResult(new { Services, ServiceTypes }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("SaveType")]
        [Authorize]
        [HttpPost]
        public IActionResult SaveType([FromForm] string title, [FromForm] string category, [FromForm] int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    ServiceType type = new ServiceType() { Title = title, Category = category, Desciption = "Desciption", IsNeedVerfication = category.Equals("1") };
                    ServiceUnit.ServiceType.Add(type);
                }
                else
                {
                    ServiceType type = ServiceUnit.ServiceType.GetUserBy(x => x.Id == id);
                    type.Title = title;
                    type.Category = category;
                    ServiceUnit.ServiceType.SaveChanges();
                }

                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetServicesTypes")]
        [HttpGet]
        public IActionResult GetServicesTypes()
        {
            try
            {
                List<ServiceType> types = ServiceUnit.ServiceType.GetAll(x => true).ToList();
                return Ok(types);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetReports")]
        [Authorize]
        [HttpGet]
        public IActionResult GetReports()
        {
            try
            {
                List<Report> reports = ServiceUnit.Report.GetAll(x => true).ToList();
                ServiceUnit.Users.GetAll(x => x.Id != null).ToList();
                return Ok(reports);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetServiceType")]
        [HttpGet]
        public IActionResult GetServiceType([FromQuery] int id)
        {
            try
            {
                ServiceType type = ServiceUnit.ServiceType.GetUserBy(x => x.Id == id);
                return Ok(type);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("RemoveServiceType")]
        [Authorize]
        [HttpPost]
        public IActionResult RemoveServiceType([FromForm] int id)
        {
            try
            {
                ServiceType type = ServiceUnit.ServiceType.GetUserBy(x => x.Id == id);
                ServiceUnit.ServiceType.RemoveObj(type);
                return Ok();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetVolunteerRequests")]
        [Authorize]
        [HttpGet]
        public IActionResult GetVolunteerRequests()
        {
            try
            {
                User user = CurrentUser;
                List<AcceptedRequest> AcceptedRequests =
                    ServiceUnit.AcceptedRequest.GetAll(x => x.VolunteerId == CurrentUser.Id).ToList();
                ServiceUnit.Users.GetAll(x => x.Id != null);
                List<Request> requests = new List<Request>();
                foreach (AcceptedRequest acr in AcceptedRequests)
                {
                    Request request = ServiceUnit.Request.GetById(acr.RequestId);
                    request.seviceType = ServiceUnit.ServiceType.GetById(request.SeviceTypeId);
                    requests.Add(request);
                }
                List<UserRating> rating = ServiceUnit.UserRating.GetAll(x => x.IsVolunteer == true).ToList();
                return Ok(new JsonResult(new { Success = true, AcceptedRequests, requests, rating }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetUserRequests")]
        [Authorize]
        [HttpGet]
        public IActionResult GetUserRequests()
        {
            try
            {
                User user = CurrentUser;
                List<Request> requests = ServiceUnit.Request.GetAll(x => x.SenderId == CurrentUser.Id).ToList(); ;
                List<AcceptedRequest> AcceptedRequests = new List<AcceptedRequest>();

                foreach (Request acr in requests)
                {
                    AcceptedRequest acceptedRequest = ServiceUnit.AcceptedRequest.GetUserBy(x => x.RequestId == acr.Id);
                    acr.seviceType = ServiceUnit.ServiceType.GetById(acr.SeviceTypeId);
                    acr.Reports = ServiceUnit.Report.Count(x => x.RequestId == acr.Id) > 0 ? new List<Report>() { new Report() { Id = 0, RequestId = acr.Id } } : null;
                    if (acceptedRequest != null)
                        acr.user = ServiceUnit.Users.GetUserBy(x => x.Id == acceptedRequest.VolunteerId);
                    else
                        acr.user = null;

                    AcceptedRequests.Add(acceptedRequest);
                }

                List<UserRating> rating = ServiceUnit.UserRating.GetAll(x => x.IsVolunteer == false).ToList();

                return Ok(new JsonResult(new { Success = true, AcceptedRequests, requests, rating }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("UserReportVolunteer")]
        [HttpPost]
        public IActionResult UserReportVolunteer([FromForm] int reqId, [FromForm] string desc)
        {
            try
            {
                User user = CurrentUser;
                AcceptedRequest request = ServiceUnit.AcceptedRequest.GetUserBy(x => x.RequestId == reqId);

                Request _req = ServiceUnit.Request.GetUserBy(x => x.Id == request.RequestId);
                _req.status = 2;
                ServiceUnit.Request.SaveChanges();

                Report _report = new Report()
                {
                    RequestId = request.RequestId,
                    UserId = user.Id,
                    UserReportedId = request.VolunteerId,
                    Description = desc,
                    Date = new DateTime(),
                    Type = 0
                };

                Report report = ServiceUnit.Report.Add(_report);

                return Ok(new JsonResult(new { Success = true, report }));
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message.ToString());
            }
        }

        [Route("UserReportAttatch")]
        [Authorize]
        [HttpPost]
        public IActionResult UserReportAttatch([FromForm] int reqId, [FromForm] string desc)
        {
            try
            {
                User user = CurrentUser;
                Service service = ServiceUnit.Service.GetUserBy(x => x.Id == reqId);

                Report _report = new Report()
                {
                    ServiceId = service.Id,
                    UserId = user.Id,
                    RequestId = null,
                    UserReportedId = service.UserId,
                    Description = desc,
                    Date = new DateTime(),
                    Type = (service.UserId == user.Id) ? 2 : 1
                };

                Report report = ServiceUnit.Report.Add(_report);

                return Ok(new JsonResult(new { Success = true, report }));
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message.ToString());
            }
        }
        [Route("UserAcceptAttatch")]
        [Authorize]
        [HttpPost]
        public IActionResult UserAcceptAttatch([FromForm] int reqId)
        {
            try
            {
                User user = CurrentUser;
                Service service = ServiceUnit.Service.GetUserBy(x => x.Id == reqId);
                ServiceType type = ServiceUnit.ServiceType.GetUserBy(x => x.Id == service.TypeId);
                Request _request = new Request()
                {
                    Date = new DateTime(),
                    SenderId = CurrentUser.Id,
                    Description = type.Desciption,
                    Lattiud = CurrentUser.Lat,
                    Longtiud = CurrentUser.Lng,
                    ExpireTime = new DateTime(),
                    SeviceTypeId = type.Id,
                    PGender = CurrentUser.Gender,
                    PDescription = null,
                    PAge = (DateTime.Today.Year - CurrentUser.BirthDate.Year),
                    PName = CurrentUser.Name,
                    VGender = CurrentUser.Gender,
                    ServiceId = service.Id,
                    status = 1,
                };

                _request = ServiceUnit.Request.Add(_request);

                AcceptedRequest acc = new AcceptedRequest()
                {
                    RequestId = _request.Id,
                    VolunteerId = service.UserId,
                    Date = new DateTime(),
                };

                ServiceUnit.AcceptedRequest.Add(acc);

                return Ok(new JsonResult(new { Success = true }));
            }
            catch (Exception e)
            {
                return BadRequest(e.InnerException.Message.ToString());
            }
        }

        [Route("DeliveredRequest")]
        [Authorize]
        [HttpPost]
        public IActionResult DeliveredRequest([FromForm] int RequestId, [FromForm] string Evaluation, [FromForm] double Rate)
        {
            try
            {
                User user = CurrentUser;

                Request req = ServiceUnit.Request.GetById(RequestId);
                req.status = 3;
                ServiceUnit.Request.SaveChanges();

                DeliveredRequest dReq = ServiceUnit.DeliveredRequest.GetAll(x => x.RequestId == RequestId).FirstOrDefault();
                if (dReq == null)
                {
                    DeliveredRequest delReqs = new DeliveredRequest()
                    {
                        RequestId = RequestId,
                        Date = new DateTime(),
                        Evaluation = Evaluation
                    };
                    ServiceUnit.DeliveredRequest.Add(delReqs);
                }

                UserRating userRate = new UserRating()
                {
                    Date = new DateTime(),
                    IsVolunteer = req.SenderId != user.Id,
                    RequestId = req.Id,
                    UserId = user.Id,
                    Description = Evaluation,
                    Value = Rate
                };
                ServiceUnit.UserRating.Add(userRate);

                if (Rate <= 2.5)
                {
                    Service _service = ServiceUnit.Service.GetUserBy(x => x.Id == req.ServiceId);
                    Report _report = new Report()
                    {
                        RequestId = req.Id,
                        UserId = user.Id,
                        UserReportedId = (req.SenderId == user.Id) ? _service.UserId : req.SenderId,
                        Description = Evaluation ?? "تقييم سيئ !",
                        Type = _service.UserId == user.Id ? 2 : 1
                    };
                    ServiceUnit.Report.Add(_report);
                }

                return Ok(new JsonResult(new { Success = true }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
        [Route("FailedRequest")]
        [Authorize]
        [HttpPost]
        public IActionResult FailedRequest([FromForm] int RequestId, [FromForm] string Reason)
        {
            try
            {
                User user = CurrentUser;

                Request req = ServiceUnit.Request.GetById(RequestId);
                req.status = 2;
                ServiceUnit.Request.SaveChanges();

                FailedRequest failReqs = new FailedRequest()
                {
                    RequestId = RequestId,
                    Date = new DateTime(),
                    Reason = Reason
                };
                ServiceUnit.FailedRequest.Add(failReqs);



                try
                {
                    Service service = ServiceUnit.Service.GetById(req.ServiceId);
                    service.IsActive = true;
                    ServiceUnit.Service.SaveChanges();
                }
                catch (Exception e) { }

                return Ok(new JsonResult(new { Success = true }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("GetServicesType")]
        [HttpGet]
        [Authorize]
        public IActionResult GetServicesType()
        {
            try
            {
                User u = CurrentUser;
                IEnumerable<ServiceType> Services = ServiceUnit.ServiceType.GetAll(x => x.Id != -1);
                return new JsonResult(new { Services });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [Route("AcceptRequest")]
        [HttpPost]
        [Authorize]
        public IActionResult AcceptRequest([FromForm] int Id)
        {
            try
            {

                Request req = ServiceUnit.Request.GetById(Id);
                Service _service = ServiceUnit.Service.GetUserBy(x => x.UserId == CurrentUser.Id && req.SeviceTypeId == x.TypeId);
                req.status = 1;
                req.ServiceId = _service.Id;
                ServiceUnit.Request.SaveChanges();

                AcceptedRequest acr = new AcceptedRequest()
                {
                    RequestId = Id,
                    Date = new DateTime(),
                    VolunteerId = CurrentUser.Id
                };
                ServiceUnit.AcceptedRequest.Add(acr);

                return Ok(new JsonResult(new { Success = true }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        //[HttpGet]
        //[ProducesResponseType(typeof(string), 200)]
        //[ProducesResponseType(500)]
        //[Route("{employeeID:int}")]
        [Route("GetRequest")]
        [HttpGet]
        [Authorize]
        public IActionResult GetRequest(int id)
        {
            try
            {
                Request _request = ServiceUnit.Request.GetById(id);
                _request.seviceType = ServiceUnit.ServiceType.GetById(_request.SeviceTypeId);
                double rate = GetUserRate(_request.SenderId);
                return Ok(new JsonResult(new { rate, request = _request }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }
        [Route("GetService")]
        [HttpGet]
        [Authorize]
        public IActionResult GetService(int id)
        {
            try
            {
                Service Service = ServiceUnit.Service.GetById(id);
                try
                {
                    List<ServiceAttachment> att = ServiceUnit.ServiceAttachment.GetAll(x => x.ServiceId == Service.Id).ToList();
                }
                catch (Exception e) { }
                return Ok(new JsonResult(new { Service }));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message.ToString());
            }
        }

        [NonAction]
        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;
            dist = dist * 1.609344;
            return dist;
        }
    }
}

