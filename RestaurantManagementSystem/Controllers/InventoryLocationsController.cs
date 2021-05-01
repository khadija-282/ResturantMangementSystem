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
    public class InventoryLocationsController : Controller
    {
        private POSEntities db = new POSEntities();

        // GET: InventoryLocations
        public async Task<ActionResult> Index()
        {
            return View(await db.InventoryLocations.ToListAsync());
        }

        // GET: InventoryLocations/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryLocation inventoryLocation = await db.InventoryLocations.FindAsync(id);
            if (inventoryLocation == null)
            {
                return HttpNotFound();
            }
            return View(inventoryLocation);
        }

        // GET: InventoryLocations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InventoryLocations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,Name,SortOrder,ShelfLocation")] InventoryLocation inventoryLocation)
        {
            if (ModelState.IsValid)
            {
                db.InventoryLocations.Add(inventoryLocation);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(inventoryLocation);
        }

        // GET: InventoryLocations/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryLocation inventoryLocation = await db.InventoryLocations.FindAsync(id);
            if (inventoryLocation == null)
            {
                return HttpNotFound();
            }
            return View(inventoryLocation);
        }

        // POST: InventoryLocations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,Name,SortOrder,ShelfLocation")] InventoryLocation inventoryLocation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventoryLocation).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inventoryLocation);
        }

        // GET: InventoryLocations/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryLocation inventoryLocation = await db.InventoryLocations.FindAsync(id);
            if (inventoryLocation == null)
            {
                return HttpNotFound();
            }
            return View(inventoryLocation);
        }

        // POST: InventoryLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            InventoryLocation inventoryLocation = await db.InventoryLocations.FindAsync(id);
            db.InventoryLocations.Remove(inventoryLocation);
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
