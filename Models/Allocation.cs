using DAL;
using Newtonsoft.Json;
using System;


namespace Models
{
    public class Allocation : Record
    {
        public Allocation()
        {
            Year = DateTime.Now.Year;
        }

        public int TeacherId { get; set; }

        public int CourseId { get; set; }

        public int Year { get; set; }

        [JsonIgnore]
        public Teacher Teacher => DB.Teachers.Get(TeacherId);

        [JsonIgnore]
        public Course Course => DB.Courses.Get(CourseId);
    }
}