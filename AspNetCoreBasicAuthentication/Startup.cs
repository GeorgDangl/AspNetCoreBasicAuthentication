using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AspNetCoreBasicAuthentication.Data;
using AspNetCoreBasicAuthentication.Models;
using AspNetCoreBasicAuthentication.Authentication;
using AspNetCoreBasicAuthentication.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace AspNetCoreBasicAuthentication
{
    public class Startup
    {
        private static readonly string _inMemoryDbName = Guid.NewGuid().ToString();

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_inMemoryDbName));

            services.AddIdentity<ApplicationUser, IdentityRole>(c =>
                {
                    c.Password.RequireDigit = false;
                    c.Password.RequiredLength = 3;
                    c.Password.RequireLowercase = false;
                    c.Password.RequireNonAlphanumeric = false;
                    c.Password.RequireUppercase = false;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddAuthentication(c =>
                {
                    c.DefaultAuthenticateScheme = "Basic";
                    c.DefaultChallengeScheme = "Basic"; // Default to basic prevents cookie auth redirect to login on 401
                })
                .AddHttpBasicAuthentication();

            services.AddMvc(options =>
            {
                // The default policy is to make sure that both authentication schemes - Cookie and Http Basic - are challenged
                var defaultPolicy = new AuthorizationPolicyBuilder(IdentityConstants.ApplicationScheme, "Basic")
                    .RequireAssertion(c => true) // A requirement is mandatory
                    .Build();
                options.Filters.Add(new AuthorizeFilter(defaultPolicy));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

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
