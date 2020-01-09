using System;
using System.Collections.Generic;

namespace lazyape.Models
{
    public static class StaticDataModel
    {
        //Holidays list
        //TODO Check the list if it is correct. 
        public static string[] Holidays =
        {
            "nyttårsdag", "palmesøndag", "skjærtorsdag", "langfredag",
            "påskedag", "kristihimmelfartsdag", "pinse",
            "juledag", "fastelavnssøndag", "mariabudskapsdag",
            "allegelgensdag", "advent", "1.mai", "grunnlovsdag", "påskeaften"
        };
        
        public static List<DayOfWeek> Weekdays = new List<DayOfWeek>
        {
            DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday
        };

        public static DateTime IgnoreDate = new DateTime(1970,1,1); //constr 1.1.1970
    }
}