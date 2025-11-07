using Microsoft.AspNetCore.Mvc;

namespace PeliculasAPIC.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
