using Microsoft.AspNetCore.Mvc;

namespace HealthCareServiceApi.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HomeController : Controller
    {
        [Route("/")]
        
       
        public IActionResult Index()
        {
            return Redirect("/swagger");
        }
    }
}
