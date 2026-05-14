using DAL;
using Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace LionelGroulx.Controllers
{
    public class TeachersController : Controller
    {
        public ActionResult Index()
        {
            var teachers = DB.Teachers.ToList()
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToList();

            return View(teachers);
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