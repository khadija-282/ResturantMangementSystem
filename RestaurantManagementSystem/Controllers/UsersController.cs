using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using RestaurantManagementSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using RestaurantManagementSystem;


namespace RestaurantManagementSystem.Controllers
{
    [CustomAuthorize("Users")]
    public class UsersController : Controller
    {
        private POSEntities db = new POSEntities();
        //private AccountController acccount = new AccountController();
        private ApplicationUserManager _userManager;
       

        public ApplicationUserManager UserManager
        {
            get { return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            private set { _userManager = value; }
        }

        [CustomAuthorize("ViewUser")]
        // GET: AspNetUsers
        public async Task<ActionResult> Index()
        {
            return View(await db.AspNetUsers.ToListAsync());
           
        }

        [CustomAuthorize("ViewUser")]
        // GET: AspNetUsers/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
           
            UserViewModel model = new UserViewModel();

            model.Email = aspNetUser.Email;
           

            foreach (AspNetRole role in aspNetUser.AspNetRoles)
            {
                model.UserSelectedRoles.Add(role.Name);
                ViewBag.Level = role.UserLevel.LevelName;
                model.LevelId = role.LevelId.Value;
            }
            var roles = db.AspNetRoles.Where(a => a.LevelId == model.LevelId);
            model.UserRoles = roles.ToList();
            model.Id = id;
            return View(model);
            
        }

        [CustomAuthorize("CreateUser")]
        // GET: AspNetUsers/Create
        public ActionResult Create()
        {
            List<UserLevel> UserLevels = db.UserLevels.ToList();           
            ViewBag.Level = UserLevels;
            UserViewModel model = new UserViewModel();
          
            return View(model);
        }

        private string GetRoleName(string RoleName)
        {
            int len = RoleName.LastIndexOf("-") > 0 ? RoleName.LastIndexOf("-") : RoleName.Length;
            return RoleName.Substring(0, len);

        }
        [HttpPost]
        public JsonResult GetRoles(int levelId, int Id)
        {
            List<SelectListItem> Roles = new List<SelectListItem>();

            var roles = db.AspNetRoles.Where(a => a.LevelId == levelId);
            // SelectListItem selectedRoles = new SelectList(roles, "Id", "Name")
            if (Id>0)
            {
                AspNetUser SelectedUser = db.AspNetUsers.Where(u => u.Id == Id).First();
                bool IsSelected = false;
                foreach (AspNetRole role in roles)
                {
                    IsSelected = false;
                    foreach (AspNetRole r in SelectedUser.AspNetRoles)
                    {
                        if (r.Name == role.Name)
                        {
                            Roles.Add(new SelectListItem() { Text = GetRoleName(role.Name), Value = role.Id.ToString(), Selected = true });
                            IsSelected = true;
                            break;
                        }
                    }
                    if (!IsSelected)
                        Roles.Add(new SelectListItem() { Text = GetRoleName(role.Name), Value = role.Id.ToString(), Selected = false });
                }
            }
            else
            {
                foreach (AspNetRole r in roles)
                    Roles.Add(new SelectListItem() { Text = GetRoleName(r.Name), Value = r.Id.ToString(), Selected = false });
                //return Json(new SelectList(roles, "Id", getRoleName()), JsonRequestBehavior.AllowGet);
            }
           
            return Json(Roles,JsonRequestBehavior.AllowGet);
        }


        [CustomAuthorize("CreateUser")]
        // POST: AspNetUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {


            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    foreach (String roleName in model.UserSelectedRoles)
                    {
                        //Assign Role to user Here 
                        await UserManager.AddToRoleAsync(user.Id, roleName);
                    }
                }
                // db.AspNetUsers.Add(aspNetUser);
                //await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                List<UserLevel> UserLevels = db.UserLevels.ToList();
                ViewBag.Level = UserLevels;
               
            }

            return View(model);
        }

        [CustomAuthorize("UpdateUser")]
        // GET: AspNetUsers/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            List<UserLevel> UserLevels = db.UserLevels.ToList();           
            ViewBag.Level = UserLevels;

            UserViewModel model = new UserViewModel();
           
            model.Email = aspNetUser.Email;
            

            foreach (AspNetRole role in aspNetUser.AspNetRoles)
            {
                model.UserSelectedRoles.Add(role.Name);
                model.LevelId = role.LevelId.Value;
            }
            var roles = db.AspNetRoles.Where(a => a.LevelId == model.LevelId);
            model.UserRoles = roles.ToList();
            model.Id = id;
            return View(model);
            //return View(aspNetUser);
        }

        [CustomAuthorize("UpdateUser")]
        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(model.Id);
                if (aspNetUser != null)
                {
                    //remove all previous roles                   

                    var roles = await UserManager.GetRolesAsync(aspNetUser.Id);
                    await UserManager.RemoveFromRolesAsync(aspNetUser.Id, roles.ToArray());

                    //add user in new roles
                    foreach (String roleName in model.UserSelectedRoles)
                    {
                        //Assign Role to user  
                        await UserManager.AddToRoleAsync(aspNetUser.Id, roleName);
                    }
                }
                db.Entry(aspNetUser).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                List<UserLevel> UserLevels = db.UserLevels.ToList();
                ViewBag.Level = UserLevels;
            }
            return View(model);
        }

        [CustomAuthorize("DeleteUser")]
        // GET: AspNetUsers/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            UserViewModel model = new UserViewModel();

            model.Email = aspNetUser.Email;            

            foreach (AspNetRole role in aspNetUser.AspNetRoles)
            {
                model.UserSelectedRoles.Add(role.Name);
                ViewBag.Level = role.UserLevel.LevelName;
                model.LevelId = role.LevelId.Value;
            }
            var roles = db.AspNetRoles.Where(a => a.LevelId == model.LevelId);
            model.UserRoles = roles.ToList();
            model.Id = id;
            return View(model);
            
        }

        [CustomAuthorize("DeleteUser")]
        // POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AspNetUser aspNetUser = await db.AspNetUsers.FindAsync(id);
            db.AspNetUsers.Remove(aspNetUser);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
 
    }
}
