using Models;
using System;
using System.Web.Mvc;

namespace LionelGroulx.Controllers
{
    public class NextSessionController : Controller
    {
        public ActionResult Edit()
        {
            ViewBag.PageTitle = "Session courante";
            ViewBag.Year = NextSession.Year;
            ViewBag.Season = NextSession.ValidSessions.Contains(1) ? "Automne" : "Hiver";

            return View();
        }

        [HttpPost]
        public ActionResult Edit(int year, string season)
        {
            if (season == "Automne")
                NextSession.CurrentDate = new DateTime(year, 2, 1);
            else
                NextSession.CurrentDate = new DateTime(year, 9, 1);

            return RedirectToAction("Index", "Students");
        }
    }
}