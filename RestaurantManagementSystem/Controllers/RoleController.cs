using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web.Mvc;
using RestaurantManagementSystem.Models;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using RestaurantManagementSystem.Toastr;

namespace RestaurantManagementSystem.Controllers
{
    [CustomAuthorize("Roles")]
    public class RoleController : Controller
    {
        private POSEntities db = new POSEntities();

        [CustomAuthorize("ViewRole")]
        // GET: Role
        public async Task<ActionResult> Index()
        {
            // var roles = db.AspNetRoles.Include(a => a.UserLevel);
            ViewBag.LevelId = new SelectList(db.UserLevels, "LevelId", "LevelName");

            long levelId = 0;
            if (db.UserLevels.Count() > 0)
                levelId = db.UserLevels.First().LevelId;

            var roles = db.AspNetRoles.Where(a => a.LevelId == levelId);

            List<RoleViewModel> rolesVM = new List<RoleViewModel>();
            foreach (AspNetRole r in roles)
            {
                RoleViewModel rVM = new RoleViewModel();
                rVM.Id = r.Id;
                rVM.Name = r.Name;
                rVM.LevelId = r.LevelId;
                rVM.LevelName = r.UserLevel.LevelName;
                rVM.setDisplayName(r.Name);
                rolesVM.Add(rVM);

            }

            return View(rolesVM);

        }

        [CustomAuthorize("ViewRole")]
        // POST: Role/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(long? LevelId)
        {
            // var roles = db.AspNetRoles.Include(a => a.UserLevel);
            ViewBag.LevelId = new SelectList(db.UserLevels, "LevelId", "LevelName");


            if (LevelId == null)
            {
                return HttpNotFound();
            }

            var roles = db.AspNetRoles.Where(a => a.LevelId == LevelId);
            List<RoleViewModel> rolesVM = new List<RoleViewModel>();
            foreach (AspNetRole r in roles)
            {
                RoleViewModel rVM = new RoleViewModel();
                rVM.Id = r.Id;
                rVM.Name = r.Name;
                rVM.LevelId = r.LevelId;
                rVM.LevelName = r.UserLevel.LevelName;
                rVM.setDisplayName(r.Name);
                rolesVM.Add(rVM);

            }
            return View(rolesVM);

        }

        [CustomAuthorize("ViewRole")]
        // GET: Role/Details/5
        public async Task<ActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRole role = await db.AspNetRoles.FindAsync(id);
            RoleViewModel roleVM = new RoleViewModel();
            if (role == null)
            {
                return HttpNotFound();
            }
            else
            {
                roleVM.Id = role.Id;
                roleVM.Name = role.Name;
                roleVM.LevelId = role.LevelId;
                roleVM.setDisplayName(role.Name);
                roleVM.LevelName = role.UserLevel.LevelName;
            }
            return View(roleVM);
        }


        [CustomAuthorize("CreateRole")]
        // GET: Role/Create
        public ActionResult Create()
        {
            ViewBag.Level = db.UserLevels;

            return View();
        }


        [CustomAuthorize("CreateRole")]
        // POST: Role/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,DisplayName,LevelName,LevelId")] RoleViewModel roleVM)
        {
            if (ModelState.IsValid)
            {
                //Guid g = Guid.NewGuid();
                //role.Id = g.ToString();
                try
                {
                    roleVM.setRoleName(roleVM.DisplayName, roleVM.LevelName);
                    AspNetRole role = new AspNetRole();
                    role.Id = roleVM.Id;
                    role.Name = roleVM.Name;
                    role.LevelId = roleVM.LevelId;

                    db.AspNetRoles.Add(role);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    if (ex is DbUpdateException)
                    {
                        if (ex.InnerException != null && ex.InnerException.InnerException != null)
                        {
                            if (ex.InnerException.InnerException is SqlException)
                            {
                                SqlException sqlException = (SqlException)ex.InnerException.InnerException;
                                switch (sqlException.Number)
                                {
                                    case 2627:  // Unique constraint error
                                    case 547:   // Constraint check violation
                                    case 2601:  // Duplicated key row error

                                        this.AddToastMessage("Error", "Role Name Already exists", ToastType.Error);
                                        break;
                                       // throw ex;
                                    default:
                                        // A custom exception of yours for other DB issues
                                        throw ex;
                                }
                            }
                        }


                    }


                }
            }
            ViewBag.Level = db.UserLevels;

            return View(roleVM);
        }

        [CustomAuthorize("UpdateRole")]
        // GET: Role/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRole role = await db.AspNetRoles.FindAsync(id);
            RoleViewModel roleVM = new RoleViewModel();
            if (role == null)
            {
                return HttpNotFound();
            }
            else
            {
                roleVM.Id = role.Id;
                roleVM.Name = role.Name;
                roleVM.LevelId = role.LevelId;
                roleVM.setDisplayName(role.Name);
                roleVM.LevelName = role.UserLevel.LevelName;
            }
            ViewBag.Level = db.UserLevels;

            return View(roleVM);
        }

        [CustomAuthorize("UpdateRole")]
        // POST: Role/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,DisplayName,LevelName,LevelId")] RoleViewModel roleVM)
        {
            if (ModelState.IsValid)
            {
                roleVM.setRoleName(roleVM.DisplayName, roleVM.LevelName);

                AspNetRole role = await db.AspNetRoles.FindAsync(roleVM.Id);
                role.Name = roleVM.Name;
                role.LevelId = roleVM.LevelId;


                db.Entry(role).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.Level = db.UserLevels;

            return View(roleVM);
        }

        [CustomAuthorize("DeleteRole")]
        // GET: Role/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetRole role = await db.AspNetRoles.FindAsync(id);
            RoleViewModel roleVM = new RoleViewModel();
            if (role == null)
            {
                return HttpNotFound();
            }
            else
            {
                roleVM.Id = role.Id;
                roleVM.Name = role.Name;
                roleVM.LevelId = role.LevelId;
                roleVM.setDisplayName(role.Name);
                roleVM.LevelName = role.UserLevel.LevelName;
            }
            return View(roleVM);
        }

        [CustomAuthorize("DeleteRole")]
        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            AspNetRole role = await db.AspNetRoles.FindAsync(id);
            db.AspNetRoles.Remove(role);
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