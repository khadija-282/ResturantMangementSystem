using System.Web;
using System.Web.Mvc;

namespace RestaurantManagementSystem
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            //filters.Add(new JsReportFilterAttribute(new LocalReporting().UseBinary(JsReportBinary.GetBinary()).AsUtility().Create()));
        }
    }
}
