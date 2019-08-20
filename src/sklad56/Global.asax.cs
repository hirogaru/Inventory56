using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using EnCore;

namespace sklad56
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public static class Globals
    {
        //достаём имена из конфига
        private static string DomNam = System.Configuration.ConfigurationManager.AppSettings["Domain_Name"].ToString();
        
        //Имена групп
        public const string DomainName = "NOVATORRU";//DomNam;

        public const string sqlGroup = DomainName + "\\sqlGroup_Inventory56_DBAdmins";

        public const string devGroup = DomainName + "\\appGroup_Inventory56_Developers";

        public const string UserGroup = DomainName + "\\appGroup_Inventory56_Users";

        public const string viewGroup = DomainName + "\\appGroup_Inventory56_Просматривающие";

        public const string editGroup = DomainName + "\\appGroup_Inventory56_Редактирующие";
        
        //Числовые значения
        public const int itemsPerPage = 5;  //Кол-во записей на странице

        public const int TeaserChars = 120;  //кол-во знаков до тизера
    }

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Bootstrapper.Run(new RegisterServicesModule());

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}