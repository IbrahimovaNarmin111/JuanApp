using System.Diagnostics;
using Juan_app.Models;
using Microsoft.AspNetCore.Mvc;

namespace Juan_app.Controllers
{
    public class HomeController : Controller
    {
        

        public IActionResult Index()
        {
            return View();
        }

       
    }
}
