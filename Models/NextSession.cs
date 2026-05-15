using System;
using System.Collections.Generic;
using System.Web;

namespace Models
{
    public static class NextSession
    {
        public static DateTime CurrentDate
        {
            get
            {
                if (HttpContext.Current.Session != null)
                {
                    if (HttpContext.Current.Session["CurrentDate"] == null)
                        HttpContext.Current.Session["CurrentDate"] = DateTime.Now;

                    return (DateTime)HttpContext.Current.Session["CurrentDate"];
                }

                return DateTime.Now;
            }
            set
            {
                HttpContext.Current.Session["CurrentDate"] = value;
            }
        }

        public static int Year
        {
            get
            {
                if (CurrentDate.Month >= 8)
                    return CurrentDate.Year + 1;

                return CurrentDate.Year;
            }
        }

        public static List<int> ValidSessions
        {
            get
            {
                if (CurrentDate.Month >= 8)
                    return new List<int> { 1, 3, 5 };

                return new List<int> { 2, 4, 6 };
            }
        }

        public static string ShortCaption
        {
            get
            {
                if (CurrentDate.Month >= 8)
                    return "Hiver " + Year;

                return "Automne " + Year;
            }
        }

        public static string Caption
        {
            get
            {
                return "Session courante : " + ShortCaption;
            }
        }
    }
}