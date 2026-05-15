using System.Linq;
using System.Web.Mvc;
using DAL;
using Models;

namespace LionelGroulx.Controllers
{
    public class CoursesController : Controller
    {
        public ActionResult Index(string search)
        {
            var courses = DB.Courses.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                courses = courses
                    .Where(c =>
                        (c.Code != null && c.Code.ToLower().Contains(search)) ||
                        (c.Title != null && c.Title.ToLower().Contains(search))
                    )
                    .ToList();
            }

            string sortBy = Session["CoursesSortBy"] as string ?? "Date";
            bool descending = Session["CoursesSortDescending"] as bool? ?? false;

            if (sortBy == "Title")
            {
                courses = descending
                    ? courses.OrderByDescending(c => c.Title).ToList()
                    : courses.OrderBy(c => c.Title).ToList();
            }
            else
            {
                courses = descending
                    ? courses.OrderByDescending(c => c.Session).ThenByDescending(c => c.Code).ToList()
                    : courses.OrderBy(c => c.Session).ThenBy(c => c.Code).ToList();
            }

            ViewBag.Search = search;

            return View(courses);
        }

        public ActionResult ToggleSearch()
        {
            Session["CoursesSearchVisible"] =
                !(Session["CoursesSearchVisible"] as bool? ?? false);

            return RedirectToAction("Index");
        }

        public ActionResult ToggleSort()
        {
            bool descending = Session["CoursesSortDescending"] as bool? ?? false;
            Session["CoursesSortDescending"] = !descending;

            return RedirectToAction("Index");
        }

        public ActionResult SetSortBy(string sortBy)
        {
            Session["CoursesSortBy"] = sortBy;

            return RedirectToAction("Index");
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