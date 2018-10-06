using Microsoft.AspNetCore.Mvc;

namespace aspnet_spa_without_indexhtml.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet("/")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            var model = new { InitialData = "something" };

            return View(model);
        }
    }
}