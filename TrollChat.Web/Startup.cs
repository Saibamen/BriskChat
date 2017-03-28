using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TrollChat.BusinessLogic.Configuration.Interfaces;
using TrollChat.DataAccess.Context;
using TrollChat.BusinessLogic.Configuration.Implementations;

namespace TrollChat.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddEntityFramework().AddDbContext<TrollChatDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Database")));

            DependencyRegister.RegisterDependecy.Register(services);

            services.AddAuthorization(options =>
            {
                options.AddPolicy("UserPolicy", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser()
                        .RequireAssertion(context => context.User.HasClaim("Role", "User"))
                        .Build();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IMigrationHelper migrationHelper)
        {
            var debugValue = Configuration.GetSection("Logging:Loglevel:Default").Value;
            var logLevel = (LogLevel)Enum.Parse(typeof(LogLevel), debugValue);

            //I'm gonna leave it as string array becase we might want to add some log modules later
            string[] logOnlyThese = { }; // or reverse string[] dontlong = {"ObjectResultExecutor", "JsonResultExecutor"};

            loggerFactory.AddDebug((category, _logLevel) => (!logOnlyThese.Any(category.Contains) && _logLevel >= logLevel));

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationScheme = "Cookies",
                LoginPath = new PathString("/Auth/Login"),
                AccessDeniedPath = new PathString("/Home/Error"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

           

            migrationHelper.Migrate();
        }
    }
}