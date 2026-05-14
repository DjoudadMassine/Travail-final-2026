using DAL;
using Models;
using System;
using System.Linq;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace LionelGroulx.Controllers
{
    public class StudentsController : Controller
    {
        // GET: Students
        public ActionResult Index()
        {
            var students = DB.Students.ToList()
                .OrderByDescending(s => s.Year)
                .ThenBy(s => s.LastName)
                .ToList();

            return View(students);
        }

        // GET: Students/Details/5
        public ActionResult Details(int id)
        {
            Student student = DB.Students.Get(id);

            if (student == null)
                return RedirectToAction("Index");

            return View(student);
        }

        // GET: Students/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        public ActionResult Create(Student student)
        {
            if (ModelState.IsValid)
            {
                // génération du code étudiant
                student.Code = GenerateStudentCode();

                DB.Students.Add(student);

                return RedirectToAction("Index");
            }

            return View(student);
        }


        public ActionResult Edit(int id)
        {
            Student student = DB.Students.Get(id);

            if (student == null)
                return RedirectToAction("Index");

            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        public ActionResult Edit(Student student)
        {
            if (ModelState.IsValid)
            {
                DB.Students.Update(student);

                return RedirectToAction("Details", new { id = student.Id });
            }

            return View(student);
        }

        // GET: Students/Delete/5
        public ActionResult Delete(int id)
        {
            DB.Students.Delete(id);

            return RedirectToAction("Index");
        }

        private string GenerateStudentCode()
        {
            Random random = new Random();

            string code;

            do
            {
                code = System.DateTime.Now.Year.ToString()
                    + random.Next(100000, 999999).ToString();
            }
            while (DB.Students.ToList().Any(s => s.Code == code));

            return code;
        }
    }
}