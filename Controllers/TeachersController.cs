using DAL;
using Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LionelGroulx.Controllers
{
    public class TeachersController : Controller
    {
        public ActionResult Index(string search)
        {
            var teachers = DB.Teachers.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                teachers = teachers
                    .Where(t =>
                        (t.Code != null && t.Code.ToLower().Contains(search)) ||
                        (t.FirstName != null && t.FirstName.ToLower().Contains(search)) ||
                        (t.LastName != null && t.LastName.ToLower().Contains(search)) ||
                        (t.Email != null && t.Email.ToLower().Contains(search))
                    )
                    .ToList();
            }

            string sortBy = Session["TeachersSortBy"] as string ?? "Title";
            bool descending = Session["TeachersSortDescending"] as bool? ?? false;

            if (sortBy == "Date")
            {
                teachers = descending
                    ? teachers.OrderByDescending(t => t.Code).ToList()
                    : teachers.OrderBy(t => t.Code).ToList();
            }
            else
            {
                teachers = descending
                    ? teachers.OrderByDescending(t => t.LastName).ThenByDescending(t => t.FirstName).ToList()
                    : teachers.OrderBy(t => t.LastName).ThenBy(t => t.FirstName).ToList();
            }

            ViewBag.Search = search;

            return View(teachers);
        }

        public ActionResult ToggleSearch()
        {
            Session["TeachersSearchVisible"] =
                !(Session["TeachersSearchVisible"] as bool? ?? false);

            return RedirectToAction("Index");
        }

        public ActionResult ToggleSort()
        {
            bool descending = Session["TeachersSortDescending"] as bool? ?? false;
            Session["TeachersSortDescending"] = !descending;

            return RedirectToAction("Index");
        }

        public ActionResult SetSortBy(string sortBy)
        {
            Session["TeachersSortBy"] = sortBy;

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);

            if (teacher == null)
                return RedirectToAction("Index");

            return View(teacher);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Create(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                teacher.Code = GenerateTeacherCode();

                DB.Teachers.Add(teacher);

                return RedirectToAction("Index");
            }

            return View(teacher);
        }

        public ActionResult Edit(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);

            if (teacher == null)
                return RedirectToAction("Index");

            return View(teacher);
        }

        [HttpPost]
        public ActionResult Edit(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                DB.Teachers.Update(teacher);

                return RedirectToAction("Details", new { id = teacher.Id });
            }

            return View(teacher);
        }

        public ActionResult Delete(int id)
        {
            DB.Teachers.Delete(id);

            return RedirectToAction("Index");
        }

        private string GenerateTeacherCode()
        {
            Random random = new Random();

            string code;

            do
            {
                code = "CLG-420-" + random.Next(10000, 99999).ToString();
            }
            while (DB.Teachers.ToList().Any(t => t.Code == code));

            return code;
        }
    }
}