using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Models
{
    public class Course : Record
    {
        public string Code { get; set; }

        public string Title { get; set; }

        // 1 à 6
        public int Session { get; set; }

        [JsonIgnore]
        public string Caption => "[" + Session + "] " + Code + " " + Title;

        [JsonIgnore]
        public List<Registration> Registrations =>
            DB.Registrations.ToList()
            .Where(r => r.CourseId == Id)
            .ToList();

        [JsonIgnore]
        public List<Student> Students
        {
            get
            {
                List<Student> students = new List<Student>();

                foreach (var registration in Registrations)
                {
                    students.Add(registration.Student);
                }

                return students;
            }
        }

        [JsonIgnore]
        public List<Allocation> Allocations =>
            DB.Allocations.ToList()
            .Where(a => a.CourseId == Id)
            .ToList();
    }
}