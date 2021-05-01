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
using System.Data.SqlClient;
using RestaurantManagementSystem.ViewModels;
using RestaurantManagementSystem.Common;

namespace RestaurantManagementSystem.Controllers
{
    [CustomAuthorize("Item")]
    public class ItemController : Controller
    {
        private POSEntities db = new POSEntities();

        [CustomAuthorize("ViewItem")]
        // GET: Items
        public async Task<ActionResult> Index()
        {
            return View();
        }
        [CustomAuthorize("ViewItem")]
        // GET: Items/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item Item = await db.Items.FindAsync(id);
            if (Item == null)
            {
                return HttpNotFound();
            }
            return View(Item);
        }

        [CustomAuthorize("CreateItem")]
        // GET: Items/Create
        public ActionResult Create()
        {
            ViewBag.Categories = db.Categories.Select(o => new { Text = o.Name, Value = o.Id}).ToList();
            return View();
        }

        [CustomAuthorize("CreateItem")]
        // POST: Items/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id, Code, Name, Description, CategoryId, ImageURL, Price, VAT, DiscountType, DiscountValue, ValidFromDate, ValidToDate, IsDealItem_YN")] Item Item)
        {
            if (ModelState.IsValid)
            {
                db.Items.Add(Item);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(Item);
        }

        [CustomAuthorize("UpdateItem")]
        // GET: Items/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item Item = await db.Items.FindAsync(id);
            if (Item == null)
            {
                return HttpNotFound();
            }
            ViewBag.Categories = db.Categories.Select(o => new { Text = o.Name, Value = o.Id }).ToList();
            return View(Item);
        }
        [CustomAuthorize("UpdateItem")]
        // POST: Items/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id, Code, Name, Description, CategoryId, ImageURL, Price, VAT, DiscountType, DiscountValue, ValidFromDate, ValidToDate, IsDealItem_YN")] Item Item)
        {
            if (ModelState.IsValid)
            {
                db.Entry(Item).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(Item);
        }

        [CustomAuthorize("DeleteItem")]
        // GET: Items/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Item Item = await db.Items.FindAsync(id);
            if (Item == null)
            {
                return HttpNotFound();
            }
            return View(Item);
        }
        [CustomAuthorize("DeleteItem")]
        // POST: Items/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Item Item = await db.Items.FindAsync(id);
            db.Items.Remove(Item);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public ActionResult GetItems(JQueryDataTableParamModel param)
        {
            int totalRecords = 0;
            //SORT Id, Code, Name, [Description], CategoryId, ImageURL, Price, VAT, DiscountType, DiscountValue, ValidFromDate, ValidToDate, IsDealItem_YN
            var sortColumnIndex = Convert.ToInt32(Request["iSortCol_0"]);
            string sortCol = (sortColumnIndex == 0 ? Convert.ToString("Id") :
                                                                 sortColumnIndex == 1 ? "Ser_No" :
                                                                 sortColumnIndex == 1 ? "Code" :
                                                                 sortColumnIndex == 3 ? "[Name]" :
                                                                 sortColumnIndex == 3 ? "[Description]" :
                                                                 sortColumnIndex == 4 ? "Category" :
                                                                 sortColumnIndex == 4 ? "IsDealItem_YN" :
                                                                 "Price");
            sortCol = string.IsNullOrEmpty(sortCol) ? "Name" : sortCol;
            string sortDir = string.IsNullOrEmpty(Request["sSortDir_0"]) ? "Asc" : Request["sSortDir_0"].ToString();

            int pageNo = param.iDisplayStart > 0 ? (param.iDisplayStart / param.iDisplayLength) + 1 : 1;
            var array = typeof(ItemViewModel).GetProperties().Where(p => p.PropertyType.Namespace == "System").Select(property => property.Name).ToArray();

            //Exclude Serial Number from coloumns list as it would be returned by DB
            string colNames = string.Join(",", array.Where(val => val != "Ser_No"));
            string ColFilterStr = CSNCommon.getColumnFilterStr(typeof(ItemViewModel), Request);
            SqlParameter prmOut = new SqlParameter("@totalRecords", SqlDbType.Int);
            prmOut.Direction = ParameterDirection.Output;

            var ContactsList = db.Database.SqlQuery<ItemViewModel>("exec [GetData] @TableName,@ColumnsName,@PageNo,@PageSize,@SortColumn,@SortOrder,@Search,@ColumnSearchQry,@totalRecords OUT ",
                                                    new SqlParameter("@TableName", "vw_Items"),
                                                    new SqlParameter("@ColumnsName", colNames),
                                                    new SqlParameter("@PageNo", pageNo),
                                                    new SqlParameter("@PageSize", param.iDisplayLength),
                                                    new SqlParameter("@SortColumn", sortCol),
                                                    new SqlParameter("@SortOrder", sortDir),
                                                    new SqlParameter("@Search", string.IsNullOrEmpty(param.sSearch) ? "" : param.sSearch),
                                                    new SqlParameter("@ColumnSearchQry", ColFilterStr),
                                                    prmOut).ToList();
            //Id, Code, Name, [Description], CategoryId, ImageURL, Price, VAT, DiscountType, DiscountValue, ValidFromDate, ValidToDate, IsDealItem_YN
            var result = from c in ContactsList
                         select new[] { Convert.ToString(c.Id), Convert.ToString(c.Ser_No), c.Code, c.Name, c.Description,c.Category,Convert.ToString(c.Price),
                             (c.IsDealItem_YN==true?"YES":"NO")};
            totalRecords = Convert.ToInt32(prmOut.Value);
            return Json(new { sEcho = param.sEcho, iTotalRecords = totalRecords, iTotalDisplayRecords = totalRecords, aaData = result }, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        // GET: Items
        public async Task<JsonResult> GetItemsByCategoryType(long categoryTypeId)
        {
            List<Item> items = new List<Item>();
            db.Configuration.ProxyCreationEnabled = false;

            if (categoryTypeId > 0)
                items = db.Items.Where(t => t.CategoryId == categoryTypeId).ToList();
            else
                items = db.Items.ToList();

            return Json(items, JsonRequestBehavior.AllowGet);
        }
        [AllowAnonymous]
        [HttpPost]
        // GET: Items
        public ActionResult GetItemByCode(string code)
        {
            db.Configuration.ProxyCreationEnabled = false;

            var items = db.Items.Where(t => t.Code == code).ToList();
            Item item = null;
            if (items.Any())
            {
                item = items.FirstOrDefault();
            }
            //return Json(item, JsonRequestBehavior.AllowGet);
            return Json(new { item = item, responseStatus = true, responseText = "Item info" }, JsonRequestBehavior.AllowGet);
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
