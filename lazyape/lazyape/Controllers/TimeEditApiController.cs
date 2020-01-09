using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using lazyape.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Task = lazyape.Models.Task;

namespace lazyape.Controllers
{
    /// <summary>
    /// TimeEdit Api Controller
    /// </summary>
    //To set it to a api controller and route it to the correct path
    [ApiController]
    [Route("api/timeedit")]
    public class TimeEditApiController : ControllerBase
    {
        //TODO make a function that select the right type, reason for better safty suring
        //TODO fix error with await on async
        
        private string baseUrlGetCourseIDs = "https://cloud.timeedit.net/uia/web/tp/objects.html?max=15&fr=t&partajax=t&im=f&sid=3&l=nb_NO&search_text=";
        private string baseUrlGetTasks = "https://cloud.timeedit.net/uia/web/tp/ri.json?h=f&sid=3&p=0.m%2C12.n&objects=";
        
        /// <summary>
        /// Get request from url with courses to get TimeEdit tasks
        /// </summary>
        /// <param name="courses">Courses it should get tasks from</param>
        /// <returns>TimeEdit tasks from courses given</returns>
        [HttpGet]
        [Route("get")]
        //URL Call example: https://localhost:5001/api/timeedit/get?courses=dat219&courses=dat202
        public async Task<IActionResult> Get([FromQuery]List<string> courses)
        {
            //Get all course IDs from all courses
            var courseIDs = GetCourseID(courses).Result;

            //Get all tasks from timeedit from all course IDs
            var timeEdits = GetTasks(courseIDs).Result;

            //Return the timeedit tasks
            return Ok(timeEdits);
        }
        
        /// <summary>
        /// Get all the tasks from TimeEdit on the course list it is given.  
        /// </summary>
        /// <param name="courses"> Courses it should get tasks from</param>
        /// <returns>TimeEdit tasks from courses given</returns>
        public async Task<List<TimeEdit>> GetAll(List<string> courses)
        {
            //Get all course IDs from all courses
            var courseIDs = GetCourseID(courses).Result;

            //Get all tasks from timeedit from all course IDs
            var timeEdits = GetTasks(courseIDs).Result;

            //Return the timeedit tasks
            return timeEdits;
        }

        /// <summary>
        ///  Get all the course IDs from the course list it is given
        /// </summary>
        /// <param name="courses"> Courses it should get course IDs for</param>
        /// <returns>Coursed IDs for the courses</returns>
        private async Task<List<string>> GetCourseID(List<string> courses)
        {
            //Course Id list
            List<string> courseIDs = new List<string>();

            //#### Get the course ID from TimeEdit ####
            foreach (var course in courses)
            {

                //Url - Get request to get the course ID
                string urlGetCourse =
                    baseUrlGetCourseIDs +
                    course + 
                    "&types=6"; // Type selector - 6 is the type for course code

                //Make a http client so we can get the html from it.
                var courseClient = new HttpClient();
                var courseHtml = await courseClient.GetStringAsync(urlGetCourse);

                //TODO Fix this better because this can work better also add more comments
                //Below
                //Take the div from the html and trim it and splitt it on "
                var divDevided = courseHtml.Trim().Split("\"");
                
                //bool for Check isDataID the next one
                bool isDataIdNext = false;
                
                //var to save dataID in
                var dataId = "";

                //Parce through list of strings to get the data
                foreach (var part in divDevided)
                {
                    //If dataID is the current place
                    if (isDataIdNext)
                    {
                        //Set the dataID
                        dataId = part;

                        //Set the dataID to false;
                        isDataIdNext = false;

                        //found it so no reason to keep going on
                        break;
                    }
                    //If the part contains data-id
                    if (part.Contains("data-id="))
                    {
                        //The next is the value
                        isDataIdNext = true;
                    }
                }
                
                //Add course ID to a the course ID list
                courseIDs.Add(dataId);
                //Above
            }
            
            //Return the course IDs
            return courseIDs;
        }

        /// <summary>
        /// Get all tasks from the course IDs list it is given
        /// </summary>
        /// <param name="courseIDs">Course IDs it should get the task for</param>
        /// <returns>Task for the course ids</returns>
        private async Task<List<TimeEdit>> GetTasks(List<string> courseIDs, string test = "")
        {
            //TimeEditList to return
            List<TimeEdit> timeEdits = new List<TimeEdit>();

            //#### Get the Tasks from TimeEdit ####
            foreach (var courseID in courseIDs)
            {
                string url = "";
                
                if (test != "")
                {
                    //test url to check
                    url = test + courseID;
                }
                else
                {
                    //Url to check to check.
                    url = baseUrlGetTasks + courseID;
                }
                
                //Make a http client so we can get the html from it.
                var client = new HttpClient();
                var html = await client.GetStringAsync(url);

                //Get the json from the root.
                var root = JObject.Parse(html);

                //Get out the properties for col, info and res.
                var col = root.Property("columnheaders");
                var info = root.Property("info");
                var res = root.Property("reservations");

                //Make a TimeEdit var to save it in
                TimeEdit te = new TimeEdit();

                //Column 
                foreach (var var in col.Value)
                {
                    //Take the object and make it to a string
                    string s = var.ToObject<string>();

                    //check if it is not null
                    if (s != null)
                    {
                        //Add it to the right list
                        te.Columnheaders.Add(s);
                    }

                }

                //Info
                foreach (var var in info.Value)
                {
                    //Take the object and make it to a string
                    int s = var.ToObject<int>();

                    //Add it to the right list
                    te.Info.Add(s);

                }

                //Reservation
                foreach (var var in res.Value)
                {
                    //Bool to check if it should add the task.
                    bool toAdd = true;

                    //Take the object of res and make it to a TimeEditReservation object 
                    TimeEdit.ReservationTimeEdit s = var.ToObject<TimeEdit.ReservationTimeEdit>();

                    //TODO Make a better checker.
                    //Check if the task is equal to a holiday
                    foreach (var day in StaticDataModel.Holidays)
                    {
                        //Trim and set it to only lower case for more right checking.
                        var tempTrim = s.columns[0].Replace(" ", "").ToLower();

                        //Check if the task is equal a holiday
                        if (tempTrim.Contains(day))
                        {
                            //If it is then stop it and set the task not to add.
                            toAdd = false;
                            break;
                        }
                    }

                    //Check if it is to add it.
                    if (toAdd)
                    {
                        //Add it to the right list and convert the to the normal reservation object
                        te.Reservations.Add(s.convertToTask());
                    }
                }

                //Adds the TimeEdit model to the list of TimeEdit models
                timeEdits.Add(te);

            }

            //return Ok(timeEdits);
            return timeEdits;
        }
        
        /// <summary>
        /// Get request for testing and displaying the TimeEdit api on the demo better
        /// </summary>
        /// <param name="courses">Courses it should get tasks from</param>
        /// <returns>TimeEdit tasks from courses given</returns>
        [HttpGet]
        [Route("get/test")]
        //URL Call example: https://localhost:5001/api/timeedit/get/test?courses=dat219&courses=dat202
        public async Task<IActionResult> GetTestUrl([FromQuery]List<string> courses)
        {
            //Get all course IDs from all courses
            var courseIDs = GetCourseID(courses).Result;

            //Get all tasks from timeedit from all course IDs
            var timeEdits = GetTasks(courseIDs, "https://cloud.timeedit.net/uia/web/tp/ri.json?h=f&sid=3&p=20190112.x%2C12.n&objects=").Result;

            //Return the timeedit tasks
            return Ok(timeEdits);
        }
        
        /// <summary>
        /// Get request for testing and displaying the TimeEdit api on the demo better
        /// </summary>
        /// <param name="courses">Courses it should get tasks from</param>
        /// <returns>TimeEdit tasks from courses given</returns>
        public async Task<List<TimeEdit>> GetAllTest(List<string> courses)
        {
            //Get all course IDs from all courses
            var courseIDs = GetCourseID(courses).Result;

            //Get all tasks from timeedit from all course IDs
            var timeEdits = GetTasks(courseIDs, "https://cloud.timeedit.net/uia/web/tp/ri.json?h=f&sid=3&p=20190112.x%2C12.n&objects=").Result;

            //Return the timeedit tasks
            return timeEdits;
        }
        
        
    }
}