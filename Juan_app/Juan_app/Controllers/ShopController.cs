using Microsoft.AspNetCore.Mvc;

namespace Juan_app.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }

    }
}
