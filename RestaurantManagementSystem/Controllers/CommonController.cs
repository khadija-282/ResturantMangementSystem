
using System.Web.Mvc;
using RestaurantManagementSystem.Models;
using Microsoft.AspNet.Identity;

namespace RestaurantManagementSystem.Controllers
{
    public class CommonController : Controller
    {
        private POSEntities db = new POSEntities();
        // GET: Common
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult getPermissions()
        {
            var currentUserId = System.Web.HttpContext.Current.User.Identity.GetUserId<int>();
            var permissions = db.GetPermissionsByUserId(currentUserId);

            return new JsonResult { Data = permissions, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}