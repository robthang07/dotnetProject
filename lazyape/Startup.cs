using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lazyape.Data;
using lazyape.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.AspNetCore.Authentication.OAuth;
using Task = System.Threading.Tasks.Task;

namespace lazyape
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }
        
        public IHostingEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            if (Environment.IsDevelopment()) // Database used during development
            {
                // Register the database context as a service. Use the SQLite for this
                services.AddDbContext<LazyApeDbContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("DefaultConnection")));
            }
            else // Database used in all other environments (production etc)
            {
                // Register the database context as a service. Use PostgreSQL server for this
                services.AddDbContext<LazyApeDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));
            }
            
            services.AddDefaultIdentity<ApplicationUser>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<LazyApeDbContext>();
            
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddAuthentication()
                .AddCookie()
                .AddOpenIdConnect("oidc", "Feide Login", options =>
                {

                    // OpenID Connect Auto configuration
                    options.MetadataAddress = "https://auth.dataporten.no/.well-known/openid-configuration";
 
                    // TODO: Store 'ClientId' and 'ClientSecret' in a safe place before deployment
                    // Link: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows
                    options.ClientId = "8c8ea59d-5631-470f-abc6-adf52f964b58";
                    options.ClientSecret = "a5267f67-c060-4377-8cf4-924f60cdc0d8";
                    
                    // Use the authorization code flow.
                    options.ResponseType = OpenIdConnectResponseType.Code;
                     
                    // Get claims from endpoint specified in auto configuration
                    options.GetClaimsFromUserInfoEndpoint = true;
                    
                    options.SaveTokens = true;
                                        
                    options.Events.OnTicketReceived = ctx =>
                    {
                        var token = new Token();
                        List<AuthenticationToken> tokens = ctx.Properties.GetTokens() as List<AuthenticationToken>;
                        tokens.Add(new AuthenticationToken() { Name = "TicketCreated", Value = DateTime.UtcNow.ToString() });
                        ctx.Properties.StoreTokens(tokens);
                        token.AccessToken = tokens[0].Value;
                        var db = ctx.HttpContext.RequestServices.GetRequiredService<LazyApeDbContext>();
                        db.Tokens.Add(token);
                        db.SaveChanges();
                        return Task.CompletedTask;
                       };
                   })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");


            });

        }
    }
}
