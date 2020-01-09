using System;
using System.Collections.Generic;
using System.Linq;
using lazyape.Data;
using lazyape.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lazyape.Controllers
{
    [ApiController]
    [Route("api/calendar")]
    public class CalendarApiController : ControllerBase
    {

        //TimeEdit Controller
        private readonly TimeEditApiController _te;
        //Auto Generator Controller
        private TaskAutoGeneratorController _auto;
        //Settings Api Controller
        private SettingsApiController _se;    
        
        //Database and user manager        
        private readonly LazyApeDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private FeideApiController _api;


        public CalendarApiController(LazyApeDbContext db, UserManager<ApplicationUser> userManager)
        {
            //Set db and user manager
            _db = db;
            _userManager = userManager;

            //Set timeEdit api controller
            _te = new TimeEditApiController();
            //Set autoGenerator controller
            _auto = new TaskAutoGeneratorController(db, userManager);
            //Set settings api controller
            _se = new SettingsApiController(db,userManager);
        }
        
 
        //TODO Should probably be a async function
        [HttpGet]
        public IActionResult GetAll()
        {
            //Variable to store the data we are going to send to the front-end.
            CalendarModel tasks = new CalendarModel();
            
            //Get course codes from the user
            List<string> courses = new List<string>();

            List<Course> courseObjects = _db.Courses.Where(w => w.UserId == _userManager.GetUserAsync(User).Result.Id).ToList();
            foreach (var obj in courseObjects)
            {
                courses.Add(obj.Code);
            }
            
            //Add TimeEdit reservation to tasks in the calendar 
            //Get all reservation from the te controller
            foreach (var res in _te.GetAll(courses).Result)
            {
                //Take out every task out from the list
                foreach (var te in res.Reservations)
                {
                    //Add the task to the te
                    tasks.AddTask(te);
                }
            }
            
            //Getting the ones from the database.
            var list = _db.Tasks.Where(w=> w.UserId == _userManager.GetUserAsync(User).Result.Id).ToList();
            foreach (var t in list)
            {
                tasks.AddTask(t);
            }
            
            //Return OK code with the task payload
            return Ok(tasks.GetTask());
        }
        
        
        [HttpGet("{id}")]
        public IActionResult Get(int id) 
        {
            //Search for the given task
            var task = _db.Tasks.Find(id);
            //task.User = _userManager.FindByIdAsync(task.UserId).Result;
                 
            //Check if the task exists, return 404 if it doesn't
            if (task == null)
                return NotFound();
               
            //return 200 Ok with the task
            return Ok(task);
        }
        
        
        [HttpPost]
        public IActionResult Post(Task task)
        {
            if (task.Id != 0)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                //Makes a connection between task and user
                task.User = _userManager.GetUserAsync(User).Result; 
                task.UserId = _userManager.GetUserAsync(User).Result.Id; 
               
                //Add task to database and saves it
                _db.Add(task);
                _db.SaveChanges();

            }
            
     
            //Return 200 ok with the new task
            return Ok(task);
                 
            //return 201 Created with the location of the new task
            //return CreatedAtAction(nameof(Get), new {id = task.Id}, task);
        }
        
        [HttpPut("{id}")]
        public IActionResult Put(Task task)
        {
            //Check if task exists
            if (!_db.Tasks.Any(p => p.Id == task.Id))
            {
                return NotFound();
            }

            task.User = _userManager.GetUserAsync(User).Result;
            task.UserId = _userManager.GetUserId(User);
                
            //Update the task in the database and saves it
            _db.Update(task);
            _db.SaveChanges();
            
            //Return 200 ok with the new task
            return Ok(task);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            //Search for the given task
            var task = _db.Tasks.Find(id);
                 
            //Check if the task exists, return 404 if it doesn't
            if (task == null)
                return NotFound();
     
            //Remove task from the database
            _db.Remove(task);
            _db.SaveChanges();
                 
            //return 200 Ok with the task
            return Ok(task);
        }
        
        [HttpGet]
        [Route("test/timeedit")]
        public IActionResult GetTestTimeEditTasks()
        {
            //Variable to store the data we are going to send to the front-end.
            CalendarModel tasks = new CalendarModel();
            
            //Get course codes from the user
            List<string> courses = new List<string>();

            List<Course> courseObjects = _db.Courses.Where(w => w.UserId == _userManager.GetUserAsync(User).Result.Id).ToList();
            foreach (var obj in courseObjects)
            {
                courses.Add(obj.Code);
            }
            
            //Add TimeEdit reservation to tasks in the calendar 
            //Get all reservation from the te controller
            foreach (var res in _te.GetAllTest(courses).Result)
            {
                //Take out every task out from the list
                foreach (var te in res.Reservations)
                {
                    //Add the task to the te
                    tasks.AddTask(te);
                }
            }

            //Return OK code with the task payload
            return Ok(tasks.GetTask());
        }

        [HttpGet]
        [Route("test/auto")]
        public IActionResult GetAutoGeneratedTasks()
        {
            //Variable to store the data we are going to send to the front-end.
            CalendarModel tasks = new CalendarModel();

            //AutoGenerate For users
            _auto.AutoGenerateTest(_userManager.GetUserId(User));

            //Get auto generated tasks from database on current user
            List<Task> te = _db.Tasks
                .Where(w => w.Type == Task.TaskType.AUTO && w.UserId == _userManager.GetUserId(User)).ToList();

            //Return OK code with the task payload
            return Ok(te);
        }

        [HttpGet]
        [Route("settings")]
        public IActionResult GetAllSettings()
        {
            return RedirectToAction("GetAll", "SettingsApi");
        }
        
        [HttpPut]
        [Route("settings/{id}")]
        public IActionResult EditSettings(Setting setting)
        {
            return RedirectToAction("EditSettings", "SettingsApi", setting);
        }
    }
}