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

namespace RestaurantManagementSystem.Controllers
{
    public class ConfigurationsController : Controller
    {
        private POSEntities db = new POSEntities();

        // GET: Configurations
        public async Task<ActionResult> Index()
        {
            return View(await db.Configurations.ToListAsync());
        }

        // GET: Configurations/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = await db.Configurations.FindAsync(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        // GET: Configurations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Configurations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,RestaurantName,Address,Phone1,Phone2,DataSync")] Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                db.Configurations.Add(configuration);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(configuration);
        }

        // GET: Configurations/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = await db.Configurations.FindAsync(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        // POST: Configurations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,RestaurantName,Address,Phone1,Phone2,DataSync")] Configuration configuration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(configuration).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(configuration);
        }

        // GET: Configurations/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Configuration configuration = await db.Configurations.FindAsync(id);
            if (configuration == null)
            {
                return HttpNotFound();
            }
            return View(configuration);
        }

        // POST: Configurations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Configuration configuration = await db.Configurations.FindAsync(id);
            db.Configurations.Remove(configuration);
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
