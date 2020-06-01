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
using GainBargain.DAL.Entities;
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


            // Parsing background job initialization

            HangfireAspNet.Use(GetHangfireServers);

            // If of the job used to identify parsing activity
            var jobId = ConfigurationManager.AppSettings["ParsingJobId"] ?? "parsing";

            // Schedule parsing as it is written in the config file
            // or at 3am UTC
            var cronStr = ConfigurationManager.AppSettings["ParsingCron"] ?? "0 3 * * *";

            RecurringJob.AddOrUpdate(jobId, () => Models.Parser.Start(), cronStr);
        }

        protected void Application_AuthenticateRequest()
        {
            GainBargainContext db = new GainBargainContext();

            if (User == null)
            {
                return;
            }

            string userName = Context.User.Identity.Name;

            string[] roles = null;

            User user = db.Users.FirstOrDefault(x => x.Email == userName);

            if (user == null)
            {
                return;
            }

            roles = db.UserRoles.Where(x => x.UserId == user.Id).Select(x => x.Role.Name).ToArray();

            IIdentity userIdentity = new GenericIdentity(userName);
            IPrincipal newUserObj = new GenericPrincipal(userIdentity, roles);

            Context.User = newUserObj;

        }

        /// <summary>
        /// This method is used for creating background
        /// tasks processors (in our case is the one for parsing).
        /// </summary>
        private IEnumerable<IDisposable> GetHangfireServers()
        {
            // Connection to the db where to store its stuff
            var conStr = ConfigurationManager.ConnectionStrings["GainBargainContext"].ConnectionString;

            // Do some magic
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

            // Create server
            yield return new BackgroundJobServer(new BackgroundJobServerOptions
            {
                // Limit max threads count to 5
                WorkerCount = Models.Parser.MAX_PROCESSING_SOURCES
            });
        }
    }
}
