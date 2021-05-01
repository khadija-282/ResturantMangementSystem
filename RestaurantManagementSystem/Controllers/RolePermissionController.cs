using RestaurantManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestaurantManagementSystem.Toastr;

namespace RestaurantManagementSystem.Controllers
{
    [CustomAuthorize("RolePermissions")]
    public class RolePermissionController : Controller
    {
        private POSEntities db = new POSEntities();
        [CustomAuthorize("ViewRolePermissions")]
        // GET: RolePermission
        public ActionResult Index()
        {
            
            List<UserLevel> UserLevels = db.UserLevels.ToList();            
            ViewBag.Level = UserLevels;

            RolePermissionViewModel model = new RolePermissionViewModel();
            model.UserLevels = UserLevels;
            return View(model);

        }
        [CustomAuthorize("UpdateRolePermissions")]
        [HttpPost]
        public JsonResult Save(string [] MenuIds, string roleId)
        {


            if (ModelState.IsValid)
            {
              string SelectedMenus =  string.Join(",", MenuIds.ToArray());
               
                db.SaveRolePermissions(roleId, SelectedMenus);
                db.SaveChanges();
                
               

            }
           
            // this.AddToastMessage("Success", "Permissions saved Successfully, yes", ToastType.Success);
            return new JsonResult { Data = MenuIds, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
            //return View();
        }

    }
}