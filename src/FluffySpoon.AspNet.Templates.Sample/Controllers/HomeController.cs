using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Templates.Sample.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITemplateRenderer _templateRenderer;

        public HomeController(
            ITemplateRenderer templateRenderer)
        {
            _templateRenderer = templateRenderer;
        }

        public async Task<IActionResult> Index()
        {
            var html = await _templateRenderer.RenderAsync(
                "MyView",
                new UserController(),
                new GroupController());
            return Content(html, "text/html");
        }
    }
}
