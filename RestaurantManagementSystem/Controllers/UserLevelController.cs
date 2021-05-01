using RestaurantManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using System.Net;

namespace RestaurantManagementSystem.Controllers
{
    [CustomAuthorize("UserLevels")]
    public class UserLevelController : Controller
    {
        private POSEntities db = new POSEntities();

        [CustomAuthorize("ViewUserLevel")]
        // GET: UserLevel
        public async Task<ActionResult> Index()
        {
            var userLevels = db.UserLevels;
            return View(await userLevels.ToListAsync());
        }


        [CustomAuthorize("ViewUserLevel")]
        // GET: UserLevel/Details/1
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLevel userLevel = await db.UserLevels.FindAsync(id);
            if (userLevel == null)
            {
                return HttpNotFound();
            }
            return View(userLevel);
        }

        [CustomAuthorize("CreateUserLevel")]
        // GET: UserLevel/Create
        public ActionResult Create()
        {
            return View();
        }

        [CustomAuthorize("CreateUserLevel")]
        // POST: UserLevel/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "LevelId,LevelName,Description,IsActive_YN")] UserLevel userLevel)
        {
            if (ModelState.IsValid)
            {
                db.UserLevels.Add(userLevel);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            
            return View(userLevel);
        }

        [CustomAuthorize("UpdateUserLevel")]
        // GET: UserLevel/Edit/1
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLevel userLevel = await db.UserLevels.FindAsync(id);
            if (userLevel == null)
            {
                return HttpNotFound();
            }
        
            return View(userLevel);
        }

        [CustomAuthorize("UpdateUserLevel")]
        // POST: UserLevel/Edit/1      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "LevelId,LevelName,Description,IsActive_YN")] UserLevel userLevel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(userLevel).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
           
            return View(userLevel);
        }

        [CustomAuthorize("DeleteUserLevel")]
        // GET: UserLevel/Delete/1
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserLevel userLevel = await db.UserLevels.FindAsync(id);
            if (userLevel == null)
            {
                return HttpNotFound();
            }
            return View(userLevel);
        }

        [CustomAuthorize("DeleteUserLevel")]
        // POST: UserLevel/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            UserLevel userLevel = await db.UserLevels.FindAsync(id);
            db.UserLevels.Remove(userLevel);
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