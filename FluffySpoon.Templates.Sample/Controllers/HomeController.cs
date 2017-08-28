using Microsoft.AspNetCore.Mvc;

namespace FluffySpoon.Templates.Sample.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
