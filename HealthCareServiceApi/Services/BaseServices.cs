using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using ModelsRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCareServiceApi.Services
{
    public class BaseServices : ControllerBase 
    {
        protected readonly IServiceUnit _serviceunit;
        public BaseServices( IServiceUnit serviceunit)
        {
            _serviceunit = serviceunit;
        }
    }
}
