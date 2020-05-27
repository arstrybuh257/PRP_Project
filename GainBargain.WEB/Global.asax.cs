using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using GainBargain.DAL.EF;
using Hangfire;
using Hangfire.SqlServer;

namespace GainBargain.WEB
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            HangfireAspNet.Use(GetHangfireServers);

            // Cron schedule
            // minutes
            // hours
            // day of the month 1-31
            // month
            // day of the week 0-6
            //RecurringJob.AddOrUpdate("parsing", () => Models.Parser.Start(), "59 8 * * *");

            // Start the parsing after 1 minute
            DateTime now = DateTime.UtcNow;
            RecurringJob.AddOrUpdate("parsing", () => Models.Parser.Start(), $"{now.Minute + 1} {now.Hour} * * *");
        }

        //protected void Application_AuthenticateRequest()
        //{
        //    GainBargainContext db = new GainBargainContext();

        //    if (User == null)
        //    {
        //        return;
        //    }

        //    string userName = Context.User.Identity.Name;

        //    string[] roles = null;

        //    User user = db.Users.FirstOrDefault(x => x.Username == userName);

        //    if (user == null)
        //    {
        //        return;
        //    }

        //    roles = db.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.Role.Name).ToArray();

        //    IIdentity userIdentity = new GenericIdentity(userName);
        //    IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

        //    Context.User = newUserObj;

        //}

        /// <summary>
        /// This method is used for creating background
        /// tasks (in our case is parsing)
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            var conStr = ConfigurationManager.ConnectionStrings["GainBargainContext"].ConnectionString;

            GlobalConfiguration.Configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(conStr, new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true,
                });

            yield return new BackgroundJobServer(new BackgroundJobServerOptions
            {
                WorkerCount = Models.Parser.MAX_PROCESSING_SOURCES
            });
        }
    }
}
