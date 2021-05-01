using RestaurantManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagementSystem.Controllers
{
    [CustomAuthorize("Permissions")]
    public class PermissionsController : Controller
    {
        public POSEntities db = new POSEntities();

        [CustomAuthorize("ViewPermission")]
        public ActionResult Index()
        {
            TreeViewModel modal = new TreeViewModel();
            return View(modal);
        }

        [CustomAuthorize("CreatePermission")]
        [HttpPost]       
        public JsonResult Add(string p_MenuId, TreeViewModel modal)
        {


            if (ModelState.IsValid)
            {
                Menu menuItem = new Menu();
                menuItem.Name = modal.text;
                menuItem.DisplayName = modal.icon;
                if (string.IsNullOrWhiteSpace(p_MenuId))
                    menuItem.Parent_Menu_Id = null;
                else
                    menuItem.Parent_Menu_Id = Convert.ToInt64(p_MenuId);
                menuItem.Id = -1;

                db.Menus.Add(menuItem);
                db.SaveChanges();
                modal.id = menuItem.Id.ToString();
               // modal.id = "200";


            }
            return new JsonResult { Data = modal, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
          //  return View(modal);
        }

        [CustomAuthorize("UpdatePermission")]
        [HttpPost]
        public ActionResult Edit(string MenuId, string newName)
        {


            if (ModelState.IsValid)
            {
                Menu menuItem = db.Menus.Find(Convert.ToInt64(MenuId));
                if (menuItem != null)
                {
                    menuItem.Name = newName;
                    menuItem.DisplayName = newName;

                    db.Entry(menuItem).State = EntityState.Modified;
                    db.SaveChangesAsync();
                }

            }

            return View();
        }

        [CustomAuthorize("DeletePermission")]
        [HttpPost]
        public ActionResult Delete(string MenuId)
        {


            if (ModelState.IsValid)
            {
                Menu menuItem = db.Menus.Find(Convert.ToInt64(MenuId));

                db.Menus.Remove(menuItem);
                db.SaveChangesAsync();

            }

            return View();
        }

     

    }
}