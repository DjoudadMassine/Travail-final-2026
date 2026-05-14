using DAL;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;


namespace Models
{
    public class Teacher : Record
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
        public List<Allocation> Allocations =>
            DB.Allocations.ToList()
            .Where(a => a.TeacherId == Id)
            .ToList();

        [JsonIgnore]
        public List<Course> Courses
        {
            get
            {
                List<Course> courses = new List<Course>();

                foreach (var allocation in Allocations)
                {
                    courses.Add(allocation.Course);
                }

                return courses;
            }
        }
    }
}