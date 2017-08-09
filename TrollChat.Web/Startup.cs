using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
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
using TrollChat.BusinessLogic.Quartz;

namespace TrollChat.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
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

            services.AddSession();

            DependencyRegister.Register(services);

            QuartzDependencyRegister.Register(services);

            AutoMapperBuilder.Build();

            services.Configure<EmailServiceCredentials>(Configuration.GetSection(nameof(EmailServiceCredentials)));

            services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
                options.Hubs.EnableJavaScriptProxies = true;
            });

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
            var logLevelParsed = (LogLevel)Enum.Parse(typeof(LogLevel), debugValue);

            // I'm gonna leave it as string array becase we might want to add some log modules later
            string[] logOnlyThese = { }; // or reverse string[] dontlong = {"ObjectResultExecutor", "JsonResultExecutor"};

            loggerFactory.AddDebug((category, logLevel) => !logOnlyThese.Any(category.Contains) && logLevel >= logLevelParsed);

            migrationHelper.Migrate();

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
            app.UseSession();

            app.UseWebSockets();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieName = CookieAuthenticationDefaults.AuthenticationScheme,
                AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
                LoginPath = new PathString("/Auth/SignIn"),
                AccessDeniedPath = new PathString("/Home/AccessDenied"),
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
            });

            app.UseSignalR();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            var scheduler = ShedulerCreator.CreateScheduler(app);
            scheduler.Start().Wait();
        }
    }
}