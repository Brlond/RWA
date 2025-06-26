using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AdminHomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public ActionResult Creation()
        {
            return View();
        }
    }
}
