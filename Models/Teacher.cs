using DAL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Models
{
    public class Teacher : Record
    {
        public string Code { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public DateTime StartDate { get; set; }

        public string Phone { get; set; }

        public string Avatar { get; set; }

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
        public List<Allocation> NextSessionAllocations =>
            Allocations
            .Where(a => a.IsNextSession)
            .ToList();

        [JsonIgnore]
        public List<Course> Courses
        {
            get
            {
                List<Course> courses = new List<Course>();

                foreach (Allocation allocation in Allocations)
                {
                    if (allocation.Course != null)
                        courses.Add(allocation.Course);
                }

                return courses;
            }
        }

        [JsonIgnore]
        public List<Course> NextSessionCourses
        {
            get
            {
                List<Course> courses = new List<Course>();

                foreach (Allocation allocation in NextSessionAllocations)
                {
                    if (allocation.Course != null)
                        courses.Add(allocation.Course);
                }

                return courses;
            }
        }

        [JsonIgnore]
        public SelectList NextSessionCoursesToSelectList =>
            SelectListUtilities<Course>.Convert(NextSessionCourses, "Caption");

        public void DeleteNextSessionAllocations()
        {
            foreach (Allocation allocation in NextSessionAllocations.ToList())
            {
                DB.Allocations.Delete(allocation.Id);
            }
        }

        public void UpdateAllocations(List<int> selectedCoursesId)
        {
            DeleteNextSessionAllocations();

            if (selectedCoursesId != null)
            {
                foreach (int courseId in selectedCoursesId)
                {
                    DB.Allocations.Add(new Allocation
                    {
                        TeacherId = Id,
                        CourseId = courseId,
                        Year = NextSession.Year
                    });
                }
            }
        }
    }
}