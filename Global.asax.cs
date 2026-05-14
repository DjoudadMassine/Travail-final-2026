using System.Globalization;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DAL;

namespace LionelGroulx
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var culture = new CultureInfo("fr-FR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Force la création des fichiers JSON
            DB.Users.ToList();
            DB.Logins.ToList();
            DB.Students.ToList();
            DB.Courses.ToList();
            DB.Teachers.ToList();
            DB.Registrations.ToList();
            DB.Allocations.ToList();
        }

        protected void Session_Start()
        {
        }

        protected void Session_End()
        {
        }

        protected void Application_End()
        {
        }
    }
}