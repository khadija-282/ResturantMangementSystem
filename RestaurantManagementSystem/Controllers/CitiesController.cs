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

    [CustomAuthorize("City")]
    public class CitiesController : Controller
    {
        private POSEntities db = new POSEntities();

        [CustomAuthorize("ViewCity")]
        // GET: Cities
        public async Task<ActionResult> Index()
        {
            var cities = db.Cities.Take(1000);//.Include(c => c.Country).Include(c => c.State).Take(100);
            return View(await cities.ToListAsync());
        }

        [CustomAuthorize("ViewCity")]
        // GET: Cities/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        [CustomAuthorize("CreateCity")]
        // GET: Cities/Create
        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name");
            ViewBag.StateId = new SelectList(db.States, "Id", "Name");
            return View();
        }
        [CustomAuthorize("UpdateCity")]
        // POST: Cities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,CountryId,StateId,IsActive_YN")] City city)
        {
            if (ModelState.IsValid)
            {
                db.Cities.Add(city);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", city.CountryId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", city.StateId);
            return View(city);
        }

        [CustomAuthorize("UpdateCity")]
        // GET: Cities/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", city.CountryId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", city.StateId);
            return View(city);
        }

        [CustomAuthorize("UpdateCity")]
        // POST: Cities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,CountryId,StateId,IsActive_YN")] City city)
        {
            if (ModelState.IsValid)
            {
                db.Entry(city).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CountryId = new SelectList(db.Countries, "Id", "Name", city.CountryId);
            ViewBag.StateId = new SelectList(db.States, "Id", "Name", city.StateId);
            return View(city);
        }

        [CustomAuthorize("DeleteCity")]
        // GET: Cities/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = await db.Cities.FindAsync(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        [CustomAuthorize("DeleteCity")]
        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            City city = await db.Cities.FindAsync(id);
            db.Cities.Remove(city);
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
