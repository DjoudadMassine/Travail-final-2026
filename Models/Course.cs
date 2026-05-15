using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class Course : Record
    {
        public string Code { get; set; }

        public string Title { get; set; }

        public int Session { get; set; }

        [JsonIgnore]
        public string Caption => "[" + Session + "] " + Code + " " + Title;

        [JsonIgnore]
        public List<Registration> Registrations =>
            DB.Registrations.ToList()
            .Where(r => r.CourseId == Id)
            .ToList();

        [JsonIgnore]
        public List<Registration> NextSessionRegistrations =>
            Registrations
            .Where(r => r.IsNextSession)
            .ToList();

        [JsonIgnore]
        public List<Student> Students
        {
            get
            {
                List<Student> students = new List<Student>();

                foreach (Registration registration in Registrations)
                {
                    if (registration.Student != null)
                        students.Add(registration.Student);
                }

                return students;
            }
        }

        [JsonIgnore]
        public List<Student> NextSessionStudents
        {
            get
            {
                List<Student> students = new List<Student>();

                foreach (Registration registration in NextSessionRegistrations)
                {
                    if (registration.Student != null)
                        students.Add(registration.Student);
                }

                return students;
            }
        }

        [JsonIgnore]
        public SelectList NextSessionStudentsToSelectList =>
            SelectListUtilities<Student>.Convert(NextSessionStudents, "Caption");

        [JsonIgnore]
        public List<Allocation> Allocations =>
            DB.Allocations.ToList()
            .Where(a => a.CourseId == Id)
            .ToList();

        public void DeleteNextSessionRegistrations()
        {
            foreach (Registration registration in NextSessionRegistrations.ToList())
            {
                DB.Registrations.Delete(registration.Id);
            }
        }

        public void UpdateRegistrations(List<int> selectedStudentsId)
        {
            DeleteNextSessionRegistrations();

            if (selectedStudentsId != null)
            {
                foreach (int studentId in selectedStudentsId)
                {
                    DB.Registrations.Add(new Registration
                    {
                        StudentId = studentId,
                        CourseId = Id,
                        Year = NextSession.Year
                    });
                }
            }
        }
    }
}