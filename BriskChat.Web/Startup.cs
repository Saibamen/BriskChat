using System;
using System.Linq;
using BriskChat.BusinessLogic.Configuration.Implementations;
using BriskChat.BusinessLogic.Configuration.Interfaces;
using BriskChat.BusinessLogic.Quartz;
using BriskChat.DataAccess.Context;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Profiling.Storage;

namespace BriskChat.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvc();

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<TrollChatDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Database")));

            services.AddSession();

            DependencyRegister.Register(services);

            QuartzDependencyRegister.Register(services);

            AutoMapperBuilder.Build();

            services.Configure<EmailServiceCredentials>(Configuration.GetSection(nameof(EmailServiceCredentials)));

            // TODO: core 2.1
            /*services.AddSignalR(options =>
            {
                options.Hubs.EnableDetailedErrors = true;
                options.Hubs.EnableJavaScriptProxies = true;
            });*/

            // If you don't want the cookie to be automatically authenticated and assigned to HttpContext.User,
            // remove the CookieAuthenticationDefaults.AuthenticationScheme parameter passed to AddAuthentication.
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Auth/SignIn";
                    options.LogoutPath = "/Account/LogOff";
                    options.AccessDeniedPath = new PathString("/Home/AccessDenied");
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

            // MiniProfiler
            services.AddMiniProfiler(options =>
            {
                // (Optional) Path to use for profiler URLs, default is /mini-profiler-resources
                options.RouteBasePath = "/profiler";

                // (Optional) Control which SQL formatter to use, InlineFormatter is the default
                options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();

                // (Optional) Control storage
                // (default is 30 minutes in MemoryCacheStorage)
                ((MemoryCacheStorage)options.Storage).CacheDuration = TimeSpan.FromMinutes(30);
            }).AddEntityFramework();
            services.AddMemoryCache();
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
                app.UseMiniProfiler();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseSession();

            app.UseWebSockets();

            app.UseAuthentication();

            // TODO: core 2.1
            //app.UseSignalR();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });


            var scheduler = SchedulerCreator.CreateScheduler(app);
            scheduler.Start().Wait();
        }
    }
}
