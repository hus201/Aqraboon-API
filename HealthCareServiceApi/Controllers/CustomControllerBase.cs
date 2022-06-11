using Microsoft.AspNetCore.Mvc;
using ModelsRepository;
using ModelsRepository.Models;
using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HealthCareServiceApi.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        protected readonly IServiceUnit ServiceUnit;
        //protected readonly ModelsRepository.Models.User CurrentUser ;
        public CustomControllerBase(IServiceUnit serviceunit)
        {
            ServiceUnit = serviceunit;
            //CurrentUser = GetCurrentUser();
        }
        public ModelsRepository.Models.User CurrentUser { get { return GetCurrentUser(); } }

        [Authorize]
        private ModelsRepository.Models.User GetCurrentUser()
        {
            try
            {
                var identity = HttpContext.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    var userClaims = identity.Claims;
                    string UserId = userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value;
                    return ServiceUnit.Users.Get(x => x.Id.ToString().Equals(UserId));
                }

            }
            catch (Exception e)
            {
                return null;
            }
            return null;
        }
    }
}
