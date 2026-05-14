using System.Web.Mvc;

namespace LionelGroulx.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Students");
        }

        public ActionResult About()
        {
            return View();
        }
    }
}