using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using lazyape.Data;
using lazyape.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;

namespace lazyape.Controllers
{
    [ApiController]
    [Route("api/feide")]
    public class FeideApiController : ControllerBase
    {
        private readonly LazyApeDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
   
        public FeideApiController(LazyApeDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;

        }


        [HttpGet]
        public IActionResult GetUserInfo()
        {
            ApplicationUserController userController = new ApplicationUserController(_db, _userManager);
            userController.ConnectUserAndTokens(_db.Users.Find(_userManager.GetUserId(User)));

            var trueToken = new Token();

            var accessToken = _db.Tokens.Where(w => w.UserId == _userManager.GetUserId(User)).ToList();
            foreach (var token in accessToken)
            {
                if (token.UserId == _userManager.GetUserId(User))
                {
                    trueToken = token;
                }
            }

            HttpClient http = new HttpClient();
            //Adding HTTP header to our Get request, the token parameter should be trueToken.AccessToken
            //However the access tokens we receive from the FEIDE login do not work
            http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", "1f4d6f53-1594-459f-a718-bbc251861e5a");
            var data = http.GetAsync("https://groups-api.dataporten.no/groups/me/groups").Result.Content
                .ReadAsStringAsync().Result;
            var tempString = data.Split(",");

            foreach (var temp in tempString)
            {

                if (temp.Contains("id"))
                {
                    var courseCode = temp.Split(":");
                    foreach (var course in courseCode)
                    {
                        if (course.Any(char.IsUpper) && course.Any(char.IsDigit))
                        {
                            Course newCourse = new Course();
                            newCourse.Code = course;
                            newCourse.UserId = _userManager.GetUserId(User);
                            //newCourse.User;
                            var courseList = _db.Courses.Where(w =>
                                w.UserId == _userManager.GetUserId(User) &&
                                w.Code == newCourse.Code).ToList();

                            if (courseList.Count == 0)
                            {
                                _db.Add(newCourse);
                            }

                        }
                    }
                }
            }



            _db.SaveChanges();

        
        return Ok();
    }
        
    }
}