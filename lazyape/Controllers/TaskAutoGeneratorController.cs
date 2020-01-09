using System;
using System.Collections.Generic;
using System.Linq;
using lazyape.Data;
using lazyape.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lazyape.Controllers
{
    [Route("auto")]
    public class TaskAutoGeneratorController : Controller
    {
        //TODO Auto check for counter up
        //TODO make it work on more treads / async / run speed
        //TODO AutoGenerateForAllUsers - Should deliver a error in some form if a error. Probably set auto gen error in table for user if cant.
        //TODO Add to static data

        private readonly LazyApeDbContext _db;
        private readonly UserManager<ApplicationUser> _um;

        public TaskAutoGeneratorController(LazyApeDbContext db, UserManager<ApplicationUser> um)
        {
            _db = db;
            _um = um;
        }

        [HttpGet]
        public void AutoGenerateForAllUsers() 
        {
            //Day before exam - currently set to 01.05
            var lastDayBeforeExam = new DateTime(DateTime.Now.Year, 5,1);
            
            //Update UserCourses
            var courseList = UpdateUserCourses();

            //Remove all auto generated tasks from date to date
            //Currently set to From 12.01 -> 01.05
            RemoveAllGeneratedTasks(new DateTime(DateTime.Now.Year, 1,12), lastDayBeforeExam);
            
            //Generate tasks on all user based on their courses.           
            GenerateTasks(courseList, new DateTime(DateTime.Now.Year, 1,12), lastDayBeforeExam);

        }

        private void RemoveAllGeneratedTasks(DateTime from, DateTime to)
        {
            //Get all Auto generated tasks from date to date
            var tasksToDelete = _db.Tasks.Where(w => w.Type == Task.TaskType.AUTO && w.Start >= from && w.Start <= to);
            
            //Remove all tasks that was found
            _db.Tasks.RemoveRange(tasksToDelete);

            //Save changes to database.
            _db.SaveChanges();
        }
        
        private void RemoveAllGeneratedTasksOnUser(DateTime from, DateTime to, string userID)
        {
            //Get all Auto generated tasks from date to date
            var tasksToDelete = _db.Tasks.Where(w => w.Type == Task.TaskType.AUTO && w.Start >= from && w.Start <= to && w.UserId == userID);
            
            //Remove all tasks that was found
            _db.Tasks.RemoveRange(tasksToDelete);

            //Save changes to database.
            _db.SaveChanges();
        }

        private Dictionary<ApplicationUser, List<Course>> UpdateUserCourses()
        {
            //GET USERS 
            //Our users
            var OurUsers = _db.Users.ToList();
            //TODO Users from Feide
            
            //GET COURSES
            //courses from our users
            var OurCourses = new Dictionary<ApplicationUser, List<Course>>();
            //TODO courses from Feide users
            
            
            //Take out get
            foreach (var user in OurUsers)
            {
                //Select Courses where Course.UserId = User.UserId && Course.Active = true
                var courses = _db.Courses.Where(w =>
                    w.UserId.Equals(user.Id) &&
                    w.Active.Equals(true)).ToList();
                
                //Add that course list to OurCourses var
                OurCourses[user] = courses;
            }
            
            //TODO Update Our Users based on Feide Users - (setUnActive, setActive, add new Active courses)
            //TODO Remove Feide User variable
            
            //Remove courses where is not active
            foreach (var key in OurCourses.Keys)
            {
                //Get courses from user (key)
                var courses = OurCourses[key];
                
                //TODO improve the removing - now it checks all courses if active if not then jump to next
                //Get each course from courses
                foreach (var course in courses)
                {
                    //Check if user is not Active
                    if (!course.Active)
                    {
                        //Remove course from courses list 
                        courses.Remove(course);
                    }
                }
            }

            //Return UpdatedUserCourses
            return OurCourses;
        }
        
        private Dictionary<ApplicationUser, List<Course>> UpdateUserCoursesOnUser(string userId)
        {
            //GET USERS 
            //Our users
            var OurUsers = _db.Users.Where(w=>w.Id== userId);
            //TODO Users from Feide
            
            //GET COURSES
            //courses from our users
            var OurCourses = new Dictionary<ApplicationUser, List<Course>>();
            //TODO courses from Feide users
            
            //Take out get
            foreach (var user in OurUsers)
            {
                //Select Courses where Course.UserId = User.UserId && Course.Active = true
                var courses = _db.Courses.Where(w =>
                    w.UserId.Equals(user.Id) &&
                    w.Active.Equals(true)).ToList();
                
                //Add that course list to OurCourses var
                OurCourses[user] = courses;
            }
            
            //TODO Update Our Users based on Feide Users - (setUnActive, setActive, add new Active courses)
            //TODO Remove Feide User variable
            
            //Remove courses where is not active
            foreach (var key in OurCourses.Keys)
            {
                //Get courses from user (key)
                var courses = OurCourses[key];
                
                //TODO improve the removing - now it checks all courses if active if not then jump to next
                //Get each course from courses
                foreach (var course in courses)
                {
                    //Check if user is not Active
                    if (!course.Active)
                    {
                        //Remove course from courses list 
                        courses.Remove(course);
                    }
                }
            }

            //Return UpdatedUserCourses
            return OurCourses;
        }

        private Dictionary<DayOfWeek, List<DateTime>> GenerateMapOfWeekdaysToExamPeriod(DateTime from, DateTime to)
        {

            //Day Lists from current day to exam for each weekdays
            Dictionary<DayOfWeek,List<DateTime>> dayList = new Dictionary<DayOfWeek, List<DateTime>>();
            
            //Make every list of days to exams
            foreach (var dayName in StaticDataModel.Weekdays)
            {
                //Set day to from day
                var day = from;
                
                //Find first day of current day
                while (day.DayOfWeek != dayName)
                {
                    day = day.AddDays(1);
                }
                
                //List To keep days in
                List<DateTime> daysForwardToExam = new List<DateTime>();

                //Go forward to exam
                while (day < to)
                {
                    //Add day
                    daysForwardToExam.Add(day);
                    //week forward
                    day = day.AddDays(7);
                }
                
                //Add all days of a specific day forward to exam.
                dayList.Add(dayName,daysForwardToExam);
            }

            //Return a map with every weekday list forward to exam.
            return dayList;
        }

        private List<DateTime> GenerateListOfWeekdaysToExamPeriod(DateTime from, DateTime to)
        {
            //List variable to store all daysForwardToExam
            List<DateTime> daysForwardToExam = new List<DateTime>();

            //Set day to current day
            //var day = currentDate;
            //Needed a day to test on 
            var day = from;

            //Loop through all days to you hit last exam day
            while (day != to)
            {
                //Check if the days is a weekday
                if (StaticDataModel.Weekdays.Contains(day.DayOfWeek))
                {
                    //Add the day to the list
                    daysForwardToExam.Add(day);
                }

                //Go a day forward
                day = day.AddDays(1);
            }
            
            //Return a list with all days forward to the exam
            return daysForwardToExam;
        }

        private Task MakeNewAutoGenerateTask(DateTime day, int hour, Course course,  ApplicationUser user)
        {
            //Make a new task
            Task newTask = new Task();

            //variables for easier to set time and date.
            var tempDay = day;
            TimeSpan taskTimeSpan = new TimeSpan(hour,0,0);
            tempDay = tempDay.Add(taskTimeSpan);
                                
            //Set start and end time
            newTask.Start = tempDay;
            newTask.End = tempDay.AddHours(1);
                                
            //Set the type to auto generated. 
            newTask.Type = Task.TaskType.AUTO;
            newTask.Title = course.Code + " - " + "AutoGen" + " - " + user.UserName;
            newTask.User = user;
            newTask.UserId = user.Id;

            //Return new task
            return newTask;
        }

        private void GenerateTasks(Dictionary<ApplicationUser, List<Course>> courseList, DateTime from, DateTime lastDayBeforeExam)
        {
            //TODO Add settings values from: startTime, endTime
            //TODO Need to take in tasks from timeEdit
            //TODO Make it so task on primary day and rest get on func since close to doup code.
            //TODO need to make all math roundings down for safty on auto gen.
            //TODO Lastly on REST check, make it if not fit on weekdays put on weekends.
            //TODO Make it so if it cant make hours need to 0, then tell user to change start and end time. 
            //TODO Make it so when adding days from rest it go after each other and not spaces. 
            //TODO Clean up code more
           
            //start time 
            var startTime = new DateTime(2019, 1,1,8,0,0);
            //end time
            var endTime = new DateTime(2019, 1,1,20,0,0);
            //inbetween time  only for hours at the moment
            var betweenTime = endTime.Hour - startTime.Hour;
            //grad C for the moment
            var grad = 300;

            //Get a list of weekdays forward to Exam Periode
            //From is set to a date instead from current date for testing.
            var dayList = GenerateMapOfWeekdaysToExamPeriod(from, lastDayBeforeExam);
            var dayList2 = GenerateListOfWeekdaysToExamPeriod(from, lastDayBeforeExam);

            //Users that we are going to do it on.
            var users = courseList.Keys;

            //For each on every user
            foreach (var user in users)
            {

                //Get how many courses the user have
                var numCourses = courseList[user].Count;
                
                //Courses the user have
                var courses = courseList[user];
                
                //TODO get TimeEdit times to check against if not cached
                //TODO Cache them so we need less request auto for it.
                
                //Get all task for the current user
                var currentUserTasks = _db.Tasks.Where(w => w.UserId == user.Id).ToList();
                
                //Hours need to reach the goal on the different courses
                Dictionary<Course,float> hoursNeedToReachTheGoal = new Dictionary<Course,float>();    
                
                //max task on a day 2 task types on a day
                int maxHoursTaskOnDay;
                if (numCourses <= 5)
                {
                    maxHoursTaskOnDay = betweenTime;
                }
                else
                {
                    maxHoursTaskOnDay = betweenTime / 2;
                }

                //List for new auto tasks
                List<Task> newTasks = new List<Task>();
                
                //Add to primary day
                var courseIndex = 0;
                foreach (var course in courses)
                {
                    
                    //Hours to reach the goal
                    hoursNeedToReachTheGoal[course] = grad - course.HoursDone;
                    
                    //Add tasks on Priority days all the way to exam periode
                    foreach (var day in dayList[StaticDataModel.Weekdays[courseIndex]])
                    {

                        var taskCounter = 0;
                        //For each hour down 
                        for (int i = 0; i <betweenTime ; i++)
                        {
                            if (hoursNeedToReachTheGoal[course] < 0 || maxHoursTaskOnDay == taskCounter)
                            {
                                break;
                            }
                            
                            //TODO Later add task so they can stick together instead of many small ones
                            
                            //First check if date is the same
                            //Check if my task start is between the start and end of a task
                            // && it is connected to the right user
                            //If it does not exist the i can add a task
                            if (!currentUserTasks.Exists(
                                e => e.Start.Day == day.Day && 
                                     e.Start.Month == day.Month &&
                                     e.Start.Hour <= (i + startTime.Hour) &&
                                     e.End.Hour >= (i + startTime.Hour) &&
                                     e.UserId == user.Id))
                            {
                                //Make a new task
                                Task newTask = MakeNewAutoGenerateTask(day, startTime.Hour  + i, course, user);

                                //Add task to new Task list
                                newTasks.Add(newTask);

                                //Remove 1 hour because we new task is 1 hour
                                hoursNeedToReachTheGoal[course] -= 1;
                                
                                //Add 1 to the task counter on this day
                                taskCounter++;

                            } 
                        }
                    }

                    courseIndex++;
                    //Reset course index to start, aka friday +1 day gives monday
                    if (courseIndex == StaticDataModel.Weekdays.Count)
                    {
                        courseIndex = 0;
                    }
                    //course.
                    
                }
                
                //Rest add here        
                foreach (var course in courses)
                {

                    //Equal placement for every course on rest days
                    maxHoursTaskOnDay = betweenTime / courses.Count;
                    
                    for (int k = 0; k < dayList2.Count ; k++)
                    {
              
                        var taskCounter = 0;
                        
                        //For each hour down 
                        for (int i = 0; i <betweenTime ; i++)
                        {
                            if (hoursNeedToReachTheGoal[course] < 0 || maxHoursTaskOnDay == taskCounter)
                            {
                                break;
                            }
                            
                            //TODO Later add task so they can stick together instead of many small ones
                            
                            //First check if date is the same
                            //Check if my task start is between the start and end of a task
                            // && it is connected to the right user
                            //If it does not exist the i can add a task
                            if (!currentUserTasks.Exists(
                                e => e.Start.Day == dayList2[k].Day &&
                                     e.Start.Month == dayList2[k].Month &&
                                     e.Start.Hour <= (i + startTime.Hour) &&
                                     e.End.Hour >= (i + startTime.Hour) &&
                                     e.UserId == user.Id)
                                && (!newTasks.Exists(
                                    e => e.Start.Day == dayList2[k].Day &&
                                     e.Start.Month == dayList2[k].Month &&
                                     e.Start.Hour <= (i + startTime.Hour) &&
                                     e.End.Hour >= (i + startTime.Hour) &&
                                     e.UserId == user.Id))
                            )
                            {
                                //Make a new task
                                Task newTask = MakeNewAutoGenerateTask(dayList2[k], startTime.Hour + i, course, user);

                                //Add task to new Task list
                                newTasks.Add(newTask);

                                //Remove 1 hour because we new task is 1 hour
                                hoursNeedToReachTheGoal[course] -= 1;
                                
                                //Add 1 to the task counter on this day
                                taskCounter++;

                            }
                        }
                    }
                }
                
                //Check for rest on all is cleared;
                foreach (var course in courses)
                {
                    if (hoursNeedToReachTheGoal[course] <= 0)
                    {
                        //TODO Add on weekends then. 
                        Console.Write("Course: " + course.Code + 
                                      ", Hours still needed for goal: " + hoursNeedToReachTheGoal[course]);
                    }
                }

                //Add new tasks to database
                foreach (var task in newTasks)
                {
                    _db.Add(task);
                }

                //Save changes to database
                _db.SaveChanges();
            }
        }
        
        [HttpGet]
        [Route("test")]
        public void AutoGenerateTest(string userId) 
        {
            //Day before exam - currently set to 01.05
            var lastDayBeforeExam = new DateTime(DateTime.Now.Year, 5,1);

            //Update UserCourses
            var courseList = UpdateUserCoursesOnUser(userId);

            //Remove all auto generated tasks from date to date
            //Currently set to From 12.01 -> 01.05
            RemoveAllGeneratedTasksOnUser(new DateTime(DateTime.Now.Year, 1,12), lastDayBeforeExam, userId);
            
            //Generate tasks on all user based on their courses.           
            GenerateTasks(courseList, new DateTime(DateTime.Now.Year, 1,12), lastDayBeforeExam);

        }
    }
}