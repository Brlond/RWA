using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class SomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
