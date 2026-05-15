using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace LionelGroulx.Controllers
{
    public class TeachersController : Controller
    {
        public ActionResult Index(string search)
        {
            ViewBag.PageTitle = "Profs";

            List<Teacher> teachers = DB.Teachers.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower();

                teachers = teachers
                    .Where(t =>
                        (t.Code != null && t.Code.ToLower().Contains(s)) ||
                        (t.FirstName != null && t.FirstName.ToLower().Contains(s)) ||
                        (t.LastName != null && t.LastName.ToLower().Contains(s)) ||
                        (t.Phone != null && t.Phone.ToLower().Contains(s))
                    )
                    .ToList();
            }

            teachers = teachers
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToList();

            ViewBag.Search = search;

            return View(teachers);
        }

        public ActionResult ToggleSearch()
        {
            Session["Search"] = !(Session["Search"] as bool? ?? false);
            return RedirectToAction("Index");
        }

        public ActionResult ToggleSort()
        {
            Session["SortAscending"] = !(Session["SortAscending"] as bool? ?? false);
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

            ViewBag.PageTitle = "Prof - Détails";

            return View(teacher);
        }

        public ActionResult Create()
        {
            ViewBag.PageTitle = "Prof - Ajout";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Teacher teacher)
        {
            if (teacher == null)
                return RedirectToAction("Index");

            teacher.Code = GenerateTeacherCode();

            DB.Teachers.Add(teacher);

            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            Teacher teacher = DB.Teachers.Get(id);

            if (teacher == null)
                return RedirectToAction("Index");

            ViewBag.PageTitle = "Prof - Modification";

            var selectedCourses = DB.Allocations.ToList()
                .Where(a =>
                    a.TeacherId == teacher.Id &&
                    a.Course != null &&
                    a.Year == NextSession.Year &&
                    NextSession.ValidSessions.Contains(a.Course.Session)
                )
                .Select(a => a.Course)
                .ToList();

            var allNextSessionCourses = DB.Courses.ToList()
                .Where(c => NextSession.ValidSessions.Contains(c.Session))
                .OrderBy(c => c.Session)
                .ThenBy(c => c.Code)
                .ToList();

            ViewBag.SelectedCourses = SelectListUtilities<Course>.Convert(selectedCourses, "Caption");
            ViewBag.Courses = SelectListUtilities<Course>.Convert(allNextSessionCourses, "Caption");

            return View(teacher);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Teacher teacher, List<int> selectedCoursesId)
        {
            Teacher storedTeacher = DB.Teachers.Get(teacher.Id);

            if (storedTeacher == null)
                return RedirectToAction("Index");

            teacher.Code = storedTeacher.Code;

            if (string.IsNullOrEmpty(teacher.Avatar))
                teacher.Avatar = storedTeacher.Avatar;

            DB.Teachers.Update(teacher);

            var oldAllocations = DB.Allocations.ToList()
                .Where(a =>
                    a.TeacherId == teacher.Id &&
                    a.Course != null &&
                    a.Year == NextSession.Year &&
                    NextSession.ValidSessions.Contains(a.Course.Session)
                )
                .ToList();

            foreach (var allocation in oldAllocations)
            {
                DB.Allocations.Delete(allocation.Id);
            }

            if (selectedCoursesId != null)
            {
                foreach (int courseId in selectedCoursesId)
                {
                    DB.Allocations.Add(new Allocation
                    {
                        TeacherId = teacher.Id,
                        CourseId = courseId,
                        Year = NextSession.Year
                    });
                }
            }

            return RedirectToAction("Details", new { id = teacher.Id });
        }

        public ActionResult Delete(int id)
        {
            List<Allocation> allocations = DB.Allocations.ToList()
                .Where(a => a.TeacherId == id)
                .ToList();

            foreach (Allocation allocation in allocations)
            {
                DB.Allocations.Delete(allocation.Id);
            }

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