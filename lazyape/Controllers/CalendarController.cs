using System.Diagnostics;
using lazyape.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace lazyape.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        // GET - Index page
        public IActionResult Index()
        {
            return View();
        }
        
        //If a error happened then show it 
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}