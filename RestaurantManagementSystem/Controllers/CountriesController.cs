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
    [CustomAuthorize("Country")]
    public class CountriesController : Controller
    {
        private POSEntities db = new POSEntities();

       [AllowAnonymous]
        // GET: Countries
        public async Task<ActionResult> Index()
        {
            return View(await db.Countries.ToListAsync());
        }

        [Authorize(Roles ="Admin")]
        // GET: Countries/Details/5

        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = await db.Countries.FindAsync(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }
        [CustomAuthorize("CreateCountry")]
        // GET: Countries/Create
        public ActionResult Create()
        {
            return View();
        }

        [CustomAuthorize("CreateCountry")]
        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Name,IsActive_YN")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Countries.Add(country);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        [CustomAuthorize("UpdateCountry")]
        // GET: Countries/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = await db.Countries.FindAsync(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        [CustomAuthorize("UpdateCountry")]
        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,IsActive_YN")] Country country)
        {
            if (ModelState.IsValid)
            {
                db.Entry(country).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = await db.Countries.FindAsync(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Country country = await db.Countries.FindAsync(id);
            db.Countries.Remove(country);
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

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetStatesByCountryId(int CountryId)
        {
            return Json(db.States.Where(t => t.CountryId == CountryId).Select(p => new { p.Id, p.Name }), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCityByStateId(int StateId)
        {
            return Json(db.Cities.Where(t => t.StateId == StateId).Select(o => new { o.Id, o.Name }), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult GetCountries()
        {
            var countries = db.Countries.Select(o=> new { o.Id, o.Name }).ToList();
            return Json(countries, JsonRequestBehavior.AllowGet);
        }
    }
}
