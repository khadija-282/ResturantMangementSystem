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
    public class InventoryItemsController : Controller
    {
        private POSEntities db = new POSEntities();

        // GET: InventoryItems
        public async Task<ActionResult> Index()
        {
            return View(await db.InventoryItems.ToListAsync());
        }

        // GET: InventoryItems/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryItem inventoryItem = await db.InventoryItems.FindAsync(id);
            if (inventoryItem == null)
            {
                return HttpNotFound();
            }
            return View(inventoryItem);
        }

        // GET: InventoryItems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InventoryItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,Name,Barcode,Description,PackagingUnit,RecipeUnit,ReciptUnitPerPackage,SortOrder,ReOrderLevel,ReplenishLevel,TotalPackages,UpdateRecipeBalance,PackagePuchasePrice,PackageSellingPrice,Group,LocationId,VendorId")] InventoryItem inventoryItem)
        {
            if (ModelState.IsValid)
            {
                db.InventoryItems.Add(inventoryItem);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(inventoryItem);
        }

        // GET: InventoryItems/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryItem inventoryItem = await db.InventoryItems.FindAsync(id);
            if (inventoryItem == null)
            {
                return HttpNotFound();
            }
            return View(inventoryItem);
        }

        // POST: InventoryItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,Name,Barcode,Description,PackagingUnit,RecipeUnit,ReciptUnitPerPackage,SortOrder,ReOrderLevel,ReplenishLevel,TotalPackages,UpdateRecipeBalance,PackagePuchasePrice,PackageSellingPrice,Group,LocationId,VendorId")] InventoryItem inventoryItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(inventoryItem).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(inventoryItem);
        }

        // GET: InventoryItems/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InventoryItem inventoryItem = await db.InventoryItems.FindAsync(id);
            if (inventoryItem == null)
            {
                return HttpNotFound();
            }
            return View(inventoryItem);
        }

        // POST: InventoryItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            InventoryItem inventoryItem = await db.InventoryItems.FindAsync(id);
            db.InventoryItems.Remove(inventoryItem);
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
