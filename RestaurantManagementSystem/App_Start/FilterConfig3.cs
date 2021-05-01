using System.Web;
using System.Web.Mvc;

namespace RestaurantManagementSystem
{
    public class FilterConfig3
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
