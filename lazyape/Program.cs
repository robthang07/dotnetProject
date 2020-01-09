using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using lazyape.Data;
using lazyape.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace lazyape
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                
                var um = services.GetRequiredService<UserManager<ApplicationUser>>();

                
                // Get our database context from the service provider
                var context = services.GetRequiredService<LazyApeDbContext>();

                // Get the environment so we can check if this is running in development or otherwise
                var environment = services.GetService<IHostingEnvironment>();
                
                // Initialise the database using the initializer from Data/ExampleDbInitializer.cs
                LazyApeDbInitializer.Initializer(context, um, environment.IsDevelopment());
               
            }    
                
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
