using RestaurantManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagementSystem.Controllers
{
    [CustomAuthorize("Log")]
    public class LogController : Controller
    {
        private POSEntities db = new POSEntities();

        [CustomAuthorize("ViewLog")]
        // GET: Log
        public ActionResult Index()
        {
            var log = db.ActivityLogs;
            return View(log);
        }

        [CustomAuthorize("ViewLog")]
        // GET: Log/Details/5
        public ActionResult Details(int id)
        {
            if (id <= 0)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ActivityLog log = db.ActivityLogs.Find(id);
           
            if (log == null)
            {
                return HttpNotFound();
            }
            
            return View(log);

        }
    }
}