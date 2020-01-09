using System;
using System.Linq;
using System.Threading.Tasks;
using lazyape.Data;
using lazyape.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace lazyape.Controllers
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsApiController : ControllerBase
    {
        private readonly LazyApeDbContext _db;
        private readonly UserManager<ApplicationUser> _um;

        public SettingsApiController(LazyApeDbContext db, UserManager<ApplicationUser> um)
        {
            _db = db;
            _um = um;
        }
        
        [HttpGet]
        [Route("get")]
        //URL Call example: https://localhost:5001/api/settings/get 
        public  IActionResult GetAll()
        {
            //Get all settings	
            var setting = _db.Settings.FirstOrDefault(w => w.UserId == _um.GetUserId(User));

            return Ok(setting);
        }

        [HttpGet]
        [Route("get/darkMode")]
        //URL Call example: https://localhost:5001/api/settings/get/darkmode 
        public IActionResult GetDarkMode()
        {
            //Get dark mode
            var setting = _db.Settings.FirstOrDefault(w => w.UserId == _um.GetUserId(User));
            
            return Ok(setting.DarkMode);
        }

        [HttpPut]
        [Route("put/{id}")]
        //URL Call example: https://localhost:5001/api/settings/put/darkmode/value
        public IActionResult EditSettings(Setting settings)
        {
            //Check if task exists
            if (!_db.Settings.Any(w => w.Id == settings.Id))
            {
                return NotFound();
            }
            
            //Update the setting
            _db.Settings.Update(settings);
            
            //Save changes
            _db.SaveChanges();

            // Return setting
            return Ok(settings);
        }

    }

}