using RestaurantManagementSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace RestaurantManagementSystem
{
    public class CustomAuthorizeAttribute : System.Web.Mvc.AuthorizeAttribute
    {
        
        public  CustomAuthorizeAttribute(params string[] roleKeys)
        {
            var currentUserId = HttpContext.Current.User.Identity.GetUserId<int>();

            //if (currentUserId > 0)
            //{


                POSEntities db = new POSEntities();
               
                var userRoles = db.GetUserRoleByUserId(currentUserId).Select(p => p.RoleId).ToArray();
                //var userCustomRoles = db.GetPermissionsByUserId(currentUserId).Select(p => new { Name = p.Permission, Role = p.Role });

                var userCustomRoles = from ma in db.MenuAccesses
                                      join m in db.Menus on ma.MenuId equals m.Id into p
                                      from subP in p.DefaultIfEmpty()
                                      join anr in db.AspNetRoles on ma.RoleId equals anr.Id into r
                                      from subr in r.DefaultIfEmpty()
                                      select new
                                      {
                                          Name = subP.Name,
                                          Role = ma.HasAccess_YN == true ? subr.Name : "None"
                                         
                                      };
               
                NameValueCollection allRoles = new NameValueCollection();
                var roles = new List<string>();
           

            foreach (var item in userCustomRoles)
                {
                    allRoles.Add(item.Name, item.Role);
                }

                foreach (var roleKey in roleKeys)
                {

                    // if (allRoles[roleKey] != null)
                    roles.AddRange(allRoles[roleKey].Split(new[] { ',' }));
                }
            roles.Add(ConfigurationManager.AppSettings["SuperAdmin"]);
            Roles = string.Join(",", roles);

              
           // }
        }
        public void CustomAuthorizeAttribute1(params string[] roleKeys)
        {
            var roles = new List<string>();
            var allRoles = (NameValueCollection)ConfigurationManager.GetSection("CustomRoles");
            foreach (var roleKey in roleKeys)
            {
                roles.AddRange(allRoles[roleKey].Split(new[] { ',' }));
            }

            Roles = string.Join(",", roles);
        }
        public override void OnAuthorization(AuthorizationContext filterContext)
        {  
        
            base.OnAuthorization(filterContext);
            if (filterContext.Result is HttpUnauthorizedResult)
            {
                if (HttpContext.Current.Request.IsAuthenticated)
                    filterContext.Result = new RedirectResult("~/Error/AccessDenied");
                else
                    filterContext.Result = new RedirectResult("~/Account/Login");
            }
        }
    }
}