using DAL;
using Newtonsoft.Json;
using System;


namespace Models
{
    public class Registration : Record
    {
        public Registration()
        {
            Year = DateTime.Now.Year;
        }

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public int Year { get; set; }

        [JsonIgnore]
        public Student Student => DB.Students.Get(StudentId);

        [JsonIgnore]
        public Course Course => DB.Courses.Get(CourseId);
    }
}