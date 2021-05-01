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
    public class RecipesController : Controller
    {
        private POSEntities db = new POSEntities();

        // GET: Recipes
        public async Task<ActionResult> Index()
        {
            return View(await db.Recipes.ToListAsync());
        }

        // GET: Recipes/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = await db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // GET: Recipes/Create
        public ActionResult Create()
        {
            ViewBag.MenuItems = new SelectList(db.Items.Select(t => new { Id = t.Id, Name = t.Name }), "Id", "Name");
            ViewBag.InvItems = new SelectList(db.InventoryItems.Select(t => new { Id = t.Id, Name = t.Name }), "Id", "Name");

            return View();
        }

        // POST: Recipes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Code,Name,MenuItemId,InventoryItemId,Quantity")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Recipes.Add(recipe);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(recipe);
        }

        // GET: Recipes/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = await db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Code,Name,MenuItemId,InventoryItemId,Quantity")] Recipe recipe)
        {
            if (ModelState.IsValid)
            {
                db.Entry(recipe).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(recipe);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> AddInventoryItem(RecipeViewModel recipeVM)
        {
            try
            {
                var recipeLineItem = new Recipe() { MenuItemId = recipeVM.MenuItemId, InventoryItemId = recipeVM.InventoryItemId, Quantity = recipeVM.Quantity };
                var addRecipeResult = db.Recipes.Add(recipeLineItem);
                var addRecipeStatus = await db.SaveChangesAsync();
                return Json(new { success = true, responseText = "Recipe item added Successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { success = false, responseText = "Recipe item could not be added." }, JsonRequestBehavior.AllowGet);
                throw;
            }
          
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<JsonResult> GetRecipeItems(long ItemId)
        {
            try
            {
                var recipeItems = from t1 in db.InventoryItems
                                  join t2 in db.Recipes on t1.Id equals t2.InventoryItemId
                                  where t2.MenuItemId==ItemId
                                  select new {t2.Id, t1.Name, t2.Quantity };
                return Json(recipeItems, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> DeleteRecipeItem(long Id)
        {
            try
            {
                var recipeItem = db.Recipes.Find(Id);
                if (recipeItem != null)
                {
                    var deleteStatus = db.Recipes.Remove(recipeItem);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Recipe item deleted successfully." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, responseText = $"Recipe item could be deleted." }, JsonRequestBehavior.AllowGet);

                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = $"{ex.Message}" }, JsonRequestBehavior.AllowGet);
                throw;
            }

        }
        // GET: Recipes/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Recipe recipe = await db.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return HttpNotFound();
            }
            return View(recipe);
        }

        // POST: Recipes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Recipe recipe = await db.Recipes.FindAsync(id);
            db.Recipes.Remove(recipe);
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
