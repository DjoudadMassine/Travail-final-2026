using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static Controllers.AccessControl;

namespace LionelGroulx.Controllers
{
    [UserAccess(Access.View)]
    public class StudentsController : Controller
    {
        public ActionResult Index(string search)
        {
            ViewBag.PageTitle = "Étudiants";

            List<Student> students = DB.Students.ToList();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.ToLower();

                students = students
                    .Where(st =>
                        (st.Code != null && st.Code.ToLower().Contains(s)) ||
                        (st.FirstName != null && st.FirstName.ToLower().Contains(s)) ||
                        (st.LastName != null && st.LastName.ToLower().Contains(s)) ||
                        (st.Email != null && st.Email.ToLower().Contains(s)))
                    .ToList();
            }

            students = students
                .OrderByDescending(st => st.Year)
                .ThenBy(st => st.LastName)
                .ThenBy(st => st.FirstName)
                .ToList();

            ViewBag.Search = search;

            return View(students);
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
            Session["StudentsSortBy"] = sortBy;
            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            Student student = DB.Students.Get(id);

            if (student == null)
                return RedirectToAction("Index");

            ViewBag.PageTitle = "Étudiant - Détails";

            return View(student);
        }
        [UserAccess(Access.Admin)]
        public ActionResult Create()
        {
            ViewBag.PageTitle = "Étudiant - Ajout";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Student student)
        {
            if (student == null)
                return RedirectToAction("Index");

            student.Code = GenerateStudentCode();

            DB.Students.Add(student);

            return RedirectToAction("Index");
        }
        [UserAccess(Access.Admin)]
        public ActionResult Edit(int id)
        {
            Student student = DB.Students.Get(id);

            if (student == null)
                return RedirectToAction("Index");

            ViewBag.PageTitle = "Étudiant - Modification";

            ViewBag.Registrations = student.NextSessionCoursesToSelectList;

            ViewBag.Courses = SelectListUtilities<Course>.Convert(
                DB.Courses.ToList()
                    .Where(c => NextSession.ValidSessions.Contains(c.Session))
                    .ToList(),
                "Caption"
            );

            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student, List<int> selectedCoursesId)
        {
            Student storedStudent = DB.Students.Get(student.Id);

            if (storedStudent == null)
                return RedirectToAction("Index");

            student.Code = storedStudent.Code;

            DB.Students.Update(student);

            student.UpdateRegistrations(selectedCoursesId);

            return RedirectToAction("Details", new { id = student.Id });
        }
        [UserAccess(Access.Admin)]
        public ActionResult Delete(int id)
        {
            Student student = DB.Students.Get(id);

            if (student != null)
            {
                student.DeleteAllRegistrations();
                DB.Students.Delete(id);
            }

            return RedirectToAction("Index");
        }
        public ActionResult GetStudents(bool forceRefresh = false)
        {
            var students = DB.Students.ToList()
                .OrderByDescending(s => s.Year)
                .ThenBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToList();

            return PartialView(students);
        }
        public ActionResult GetStudentDetails(int id, bool forceRefresh = false)
        {
            Student student = DB.Students.Get(id);

            if (student == null)
                return null;

            return PartialView(student);
        }
        private string GenerateStudentCode()
        {
            Random random = new Random();
            string code;

            do
            {
                code = DateTime.Now.Year.ToString() + random.Next(100000, 999999).ToString();
            }
            while (DB.Students.ToList().Any(s => s.Code == code));

            return code;
        }
    }
}