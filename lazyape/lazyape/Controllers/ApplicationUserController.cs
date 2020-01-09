using System.Linq;
using lazyape.Data;
using lazyape.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Design;

namespace lazyape.Controllers
{
    public class ApplicationUserController : ControllerBase
    {
        private LazyApeDbContext _db;
        private UserManager<ApplicationUser> _userManager;

        public ApplicationUserController(LazyApeDbContext db, UserManager<ApplicationUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        
        public void ConnectUserAndTokens(ApplicationUser user)
        {

            var count = _db.Tokens.Count();
            var token = _db.Tokens.Find(count);
            token.UserId = user.Id;
            _db.SaveChanges();
        }
    }
}