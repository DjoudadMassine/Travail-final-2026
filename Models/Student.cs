using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;


namespace Models
{
    public class Student : Record
    {
        public string Code { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        [JsonIgnore]
        public string FullName => LastName + " " + FirstName;

        [JsonIgnore]
        public string Caption => Code + " " + LastName + " " + FirstName;

        [JsonIgnore]
        public int Year => int.Parse(Code.Substring(0, 4));

        [JsonIgnore]
        public List<Registration> Registrations =>
            DB.Registrations.ToList()
            .Where(r => r.StudentId == Id)
            .ToList();

        [JsonIgnore]
        public List<Course> Courses
        {
            get
            {
                List<Course> courses = new List<Course>();

                foreach (var registration in Registrations)
                {
                    courses.Add(registration.Course);
                }

                return courses;
            }
        }
    }
}