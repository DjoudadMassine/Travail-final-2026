using System.Linq;
using System.Web.Mvc;
using DAL;
using Models;

namespace LionelGroulx.Controllers
{
    public class CoursesController : Controller
    {
        public ActionResult Index()
        {
            var courses = DB.Courses.ToList()
                .OrderBy(c => c.Session)
                .ThenBy(c => c.Code)
                .ToList();

            return View(courses);
        }

        public ActionResult Details(int id)
        {
            Course course = DB.Courses.Get(id);

            if (course == null)
                return RedirectToAction("Index");

            return View(course);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                DB.Courses.Add(course);
                return RedirectToAction("Index");
            }

            return View(course);
        }

        public ActionResult Edit(int id)
        {
            Course course = DB.Courses.Get(id);

            if (course == null)
                return RedirectToAction("Index");

            return View(course);
        }

        [HttpPost]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                DB.Courses.Update(course);
                return RedirectToAction("Details", new { id = course.Id });
            }

            return View(course);
        }

        public ActionResult Delete(int id)
        {
            DB.Courses.Delete(id);
            return RedirectToAction("Index");
        }
    }
}