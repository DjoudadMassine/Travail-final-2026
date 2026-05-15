using DAL;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Controllers.AccessControl;

namespace LionelGroulx.Controllers
{
    [UserAccess(Access.View)]
    public class CoursesController : Controller
    {
        public ActionResult Index(string search)
        {
            ViewBag.PageTitle = "Cours";

            List<Course> courses = DB.Courses.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower();

                courses = courses
                    .Where(c =>
                        (c.Code != null && c.Code.ToLower().Contains(s)) ||
                        (c.Title != null && c.Title.ToLower().Contains(s))
                    )
                    .ToList();
            }

            courses = courses
                .OrderBy(c => c.Session)
                .ThenBy(c => c.Code)
                .ToList();

            ViewBag.Search = search;

            return View(courses);
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
            Session["CoursesSortBy"] = sortBy;
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Course course = DB.Courses.Get(id);

            if (course == null)
                return RedirectToAction("Index");

            ViewBag.PageTitle = "Cours - Détails";

            return View(course);
        }
        [UserAccess(Access.Write)]
        public ActionResult Create()
        {
            ViewBag.PageTitle = "Cours - Ajout";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Course course)
        {
            if (course == null)
                return RedirectToAction("Index");

            DB.Courses.Add(course);

            return RedirectToAction("Index");
        }
        [UserAccess(Access.Write)]
        public ActionResult Edit(int id)
        {
            Course course = DB.Courses.Get(id);

            if (course == null)
                return RedirectToAction("Index");

            ViewBag.PageTitle = "Cours - Modification";

            List<int> selectedStudentIds = DB.Registrations.ToList()
                .Where(r =>
                    r.CourseId == course.Id &&
                    r.Student != null &&
                    r.Year == NextSession.Year &&
                    NextSession.ValidSessions.Contains(course.Session)
                )
                .Select(r => r.StudentId)
                .ToList();

            List<Student> allStudents = DB.Students.ToList()
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToList();

            List<Student> selectedStudents = allStudents
                .Where(s => selectedStudentIds.Contains(s.Id))
                .ToList();

            List<Student> availableStudents = allStudents
                .Where(s => !selectedStudentIds.Contains(s.Id))
                .ToList();

            ViewBag.SelectedStudents = SelectListUtilities<Student>.Convert(selectedStudents, "Caption");
            ViewBag.Students = SelectListUtilities<Student>.Convert(availableStudents, "Caption");

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course course, List<int> selectedStudentsId)
        {
            if (course == null)
                return RedirectToAction("Index");

            Course storedCourse = DB.Courses.Get(course.Id);

            if (storedCourse == null)
                return RedirectToAction("Index");

            DB.Courses.Update(course);

            List<Registration> oldRegistrations = DB.Registrations.ToList()
                .Where(r =>
                    r.CourseId == course.Id &&
                    r.Year == NextSession.Year
                )
                .ToList();

            foreach (Registration registration in oldRegistrations)
            {
                DB.Registrations.Delete(registration.Id);
            }

            if (selectedStudentsId != null)
            {
                foreach (int studentId in selectedStudentsId)
                {
                    DB.Registrations.Add(new Registration
                    {
                        StudentId = studentId,
                        CourseId = course.Id,
                        Year = NextSession.Year
                    });
                }
            }

            return RedirectToAction("Details", new { id = course.Id });
        }
        [UserAccess(Access.Write)]
        public ActionResult Delete(int id)
        {
            List<Registration> registrations = DB.Registrations.ToList()
                .Where(r => r.CourseId == id)
                .ToList();

            foreach (Registration registration in registrations)
            {
                DB.Registrations.Delete(registration.Id);
            }

            List<Allocation> allocations = DB.Allocations.ToList()
                .Where(a => a.CourseId == id)
                .ToList();

            foreach (Allocation allocation in allocations)
            {
                DB.Allocations.Delete(allocation.Id);
            }

            DB.Courses.Delete(id);

            return RedirectToAction("Index");
        }

        public ActionResult GetCourses(bool forceRefresh = false)
        {
            var courses = DB.Courses.ToList()
                .OrderBy(c => c.Session)
                .ThenBy(c => c.Code)
                .ToList();

            return PartialView(courses);
        }
        public ActionResult GetCourseDetails(int id, bool forceRefresh = false)
        {
            Course course = DB.Courses.Get(id);

            if (course == null)
                return null;

            return PartialView(course);
        }

    }
}