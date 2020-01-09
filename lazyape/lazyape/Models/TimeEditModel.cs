using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace lazyape.Models
{   
    public class TimeEdit
    {
        public class ReservationTimeEdit
        {
            [JsonProperty("id")]
            public int id { get; set; }
            
            [JsonProperty("startdate")]
            public string startDate { get; set; }
            
            [JsonProperty("starttime")]
            public string startTime { get; set; }
            
            [JsonProperty("enddate")]
            public string endDate { get; set; }
            
            [JsonProperty("endtime")]
            public string endTime { get; set; }
            
            [JsonProperty("columns")]
            public List<string> columns { get; set; }

            public Task convertToTask()
            {
                Task task = new Task();

                //Convert the start and end date to the right format
                DateTime sDate = Convert.ToDateTime(startDate + " " + startTime);
                DateTime eDate = Convert.ToDateTime(endDate + " " + endTime);
                
                //Add the start and end to the new reservation
                task.Start = Convert.ToDateTime(sDate.ToString("yyyy-MM-dd") + " " + startTime);
                task.End = Convert.ToDateTime(eDate.ToString("yyyy-MM-dd") + " " + endTime);

                //TODO Make this pick the right 1 automatic
                //Set the title on the task
                task.Title = columns[0] + " - " + columns[5] + " - " + columns[2];
                
                return task;
            }
        }
        
        //Below is the TimeEdit elements
        [JsonProperty("columnheaders")]
        public List<string> Columnheaders { get; set; }
        
        [JsonProperty("info")]
        public List<int> Info { get; set; }
        
        [JsonProperty("reservations")]
        public List<Task> Reservations { get; set; }

        public TimeEdit()
        {
            //Make the new lists
            Columnheaders = new List<string>();
            Info = new List<int>();
            Reservations = new List<Task>();
        }
    }
}