using System;
using System.Collections.Generic;
using System.Text;
using lazyape.Models;
using Microsoft.AspNetCore.Authentication.Twitter;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace lazyape.Data
{
    public class LazyApeDbContext : IdentityDbContext<ApplicationUser>
    {
        public LazyApeDbContext(DbContextOptions<LazyApeDbContext> options)
            : base(options)
        {
        }
        
        //Have created a table in db to store users setting preference 
        public DbSet<Setting> Settings { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Course> Courses { get; set; } 
        
        public DbSet<Token> Tokens { get; set; }
       
        
    }
}
