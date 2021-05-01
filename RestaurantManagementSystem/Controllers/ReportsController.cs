using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagementSystem.Controllers
{
    public class ReportsController : Controller
    {
        private POSEntities db = new POSEntities();
        [HttpGet]
        [AllowAnonymous]
        //[EnableJsReport()]
        public ActionResult ShowJSReport()
        {
           // HttpContext.JsReportFeature().Recipe(jsreport.Types.Recipe.ChromePdf);

            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult SalesByDate(DateTime? fromDate, DateTime? toDate,string shift="All", bool cancelledOrders=false)
        {
            ViewBag.StatusList = db.Status.ToList();
            var fromDateValue = fromDate ?? (DateTime.Now.AddDays(-30));
            var toDateValue = toDate ?? DateTime.Now;
            var shiftValue = (shift.Equals("All") ? "" : $" AND Shift='{shift}'");
            var cancelledOrdersQuery = (cancelledOrders ? " AND o.Status=6" : "");
            var invoiceTable = (cancelledOrders ? "SCWTimer" : "Invoice");
            string sqlNew = $@" SELECT ISNULL(i.Id, 0)     Id,o.Code,o.Shift, CAST(ISNULL(i.InvoiceNumber, 0) AS NVARCHAR) InvoiceNumber,ISNULL(o.BillNumber,'0') BillNumber,
                                   CASE 
                                        WHEN i.InvoiceDate IS NULL THEN ''
                                         ELSE CAST(Convert(nvarchar,i.InvoiceDate,107) AS NVARCHAR)
                                   END                 InvoiceDate,
                                   o.Shift,
                                   CAST(0 as NVARCHAR)                AS Pax,
                                    CAST(
                                    CAST((ISNULL(i.NetTotal, 0) -ISNULL(i.GST, 0) -((ISNULL(i.ServiceCharges, 0) / 100.0) * ISNULL(i.GrandTotal, 0))) 
                                    + ( (ISNULL(i.DiscountValue, 0) / 100.0) * ISNULL(i.GrandTotal, 0)) AS DECIMAL(18, 2)) AS NVARCHAR) Amount,
                                    CAST(CAST(ISNULL(i.GST, 0) AS DECIMAL(18, 2)) AS NVARCHAR) GST,
                                    CAST(CAST( (ISNULL(i.ServiceCharges, 0) / 100) * ISNULL(i.GrandTotal, 0) AS DECIMAL(18, 2)) AS NVARCHAR ) SC,
                                    CAST(CAST((ISNULL(i.DiscountValue, 0) / 100) * ISNULL(i.GrandTotal, 0)AS DECIMAL(18, 2)) AS NVARCHAR) Discount,
                                    CAST(ISNULL(i.NetTotal, 0) AS NVARCHAR) Received,
                                   o.Location
                            FROM   {invoiceTable}          AS i
                                   INNER JOIN [Order] o
                                        ON  i.OrderId = o.Id
                            WHERE  CONVERT(VARCHAR(10), i.InvoiceDate, 112) >= CONVERT(VARCHAR(10), CAST('{fromDateValue.Date}' AS DATETIME), 112)
                                   AND CONVERT(VARCHAR(10), i.InvoiceDate, 112) <= CONVERT(VARCHAR(10), CAST('{toDateValue.Date}' AS DATETIME), 112) {shiftValue}";
                     var orders = db.Database.SqlQuery<DatewiseSalesViewModel>(sqlNew);
            return View(orders);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ItemWiseSales(DateTime? fromDate, DateTime? toDate,string shift="All")
        {
            
            ViewBag.StatusList = db.Status.ToList();
            var fromDateValue = fromDate ?? (DateTime.Now.AddDays(-30));
            var toDateValue = toDate ?? DateTime.Now;
            var shiftValue = (shift.Equals("All") ? "" : $" AND Shift='{shift}'");
            string sql = $@"SELECT i2.Code ItemCode, i2.Name ItemName,o.Shift, ISNULL(c.Code,'N/A') CategoryCode, ISNULL(c.Name,'N/A') Category, SUM(ISNULL(od.Quantity, 0))     Quantity
                            FROM   Invoice AS i INNER JOIN [Order]           AS o
                                        ON  o.Id = i.OrderId INNER JOIN OrderDetail       AS od
                                        ON  od.OrderId = o.Id INNER JOIN Item              AS i2
                                        ON  i2.Id = od.ItemId LEFT JOIN Category          AS c
                                        ON  c.Id = i2.CategoryId
                            WHERE  CONVERT(VARCHAR(10), i.InvoiceDate, 112) >= CONVERT(VARCHAR(10), CAST('{fromDateValue.Date}' AS DATETIME), 112)
                            AND CONVERT(VARCHAR(10), i.InvoiceDate, 112) <= CONVERT(VARCHAR(10), CAST('{toDateValue.Date}' AS DATETIME), 112)
                            {shiftValue}
                            GROUP BY od.ItemId, i2.Code, i2.Name,o.Shift, c.Code, c.Name";
            var itemWiseSales = db.Database.SqlQuery<ItemWiseSales>(sql);
            return View(itemWiseSales);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult CategoryWiseSales(DateTime? fromDate, DateTime? toDate)
        {

            ViewBag.StatusList = db.Status.ToList();
            var fromDateValue = fromDate ?? (DateTime.Now.AddDays(-30));
            var toDateValue = toDate ?? DateTime.Now;
            string sql = $@"SELECT c.Code Code, c.Name Category, SUM(ISNULL(od.Quantity, 0))     Quantity,COUNT(i.OrderId) NoOfOrders
                            FROM   Invoice AS i INNER JOIN [Order]           AS o
                                        ON  o.Id = i.OrderId INNER JOIN OrderDetail       AS od
                                        ON  od.OrderId = o.Id INNER JOIN Item              AS i2
                                        ON  i2.Id = od.ItemId INNER JOIN Category          AS c
                                        ON  c.Id = i2.CategoryId
                            WHERE  CONVERT(VARCHAR(10), i.InvoiceDate, 112) >= CONVERT(VARCHAR(10), CAST('{fromDateValue.Date}' AS DATETIME), 112)
                            AND CONVERT(VARCHAR(10), i.InvoiceDate, 112) <= CONVERT(VARCHAR(10), CAST('{toDateValue.Date}' AS DATETIME), 112)
                            GROUP BY od.ItemId, i2.Code, i2.Name, i2.CategoryId, c.Code, c.Name";
            var categoryWiseSales = db.Database.SqlQuery<CategoryWiseSales>(sql);
            return View(categoryWiseSales);
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult ShiftWiseSales(DateTime? fromDate, DateTime? toDate)
        {

            ViewBag.StatusList = db.Status.ToList();
            var fromDateValue = fromDate ?? (DateTime.Now.AddDays(-30));
            var toDateValue = toDate ?? DateTime.Now;
            string sql = $@"SELECT o.Shift,COUNT(i.OrderId) NoOfOrders
                            FROM   Invoice AS i INNER JOIN [Order]           AS o
                                        ON  o.Id = i.OrderId INNER JOIN OrderDetail       AS od
                                        ON  od.OrderId = o.Id INNER JOIN Item              AS i2
                                        ON  i2.Id = od.ItemId INNER JOIN Category          AS c
                                        ON  c.Id = i2.CategoryId
                            WHERE  CONVERT(VARCHAR(10), i.InvoiceDate, 112) >= CONVERT(VARCHAR(10), CAST('{fromDateValue.Date}' AS DATETIME), 112)
                            AND CONVERT(VARCHAR(10), i.InvoiceDate, 112) <= CONVERT(VARCHAR(10), CAST('{toDateValue.Date}' AS DATETIME), 112)
                            GROUP BY o.Shift";
            var categoryWiseSales = db.Database.SqlQuery<CategoryWiseSales>(sql);
            return View(categoryWiseSales);
        }
        [HttpGet]
        [CustomAuthorize("Order")]
        public ActionResult Dashboard(DateTime? fromDate, DateTime? toDate)
        {

            DashboardViewModel dashboard = new DashboardViewModel();
            string monthlySalesQuery = $@"SELECT Sum(ISNULL(i.NetTotal, 0)) Sales,Sum(ISNULL(i.GST, 0)) GST,Sum(ISNULL(i.ServiceCharges, 0)) ServiceCharges FROM Invoice AS i
                                     WHERE Month(i.InvoiceDate) = MONTH(GETDATE()) AND YEAR(i.InvoiceDate) = YEAR(GETDATE())";

            string dailySalesQuery = $@"SELECT Sum(ISNULL(i.NetTotal, 0)) Sales,Sum(ISNULL(i.GST, 0)) GST, Sum(ISNULL(i.ServiceCharges, 0)) ServiceCharges 
                                    FROM Invoice AS i WHERE  CONVERT(varchar, i.InvoiceDate, 112) = convert(varchar, GETDATE(), 112)";

            string todayHotItemQuery = $@"SELECT top 1 od.ItemId Id,i.Code, i.Name, SUM(od.Quantity) Quantity 
                                    FROM [ORDER] o INNER JOIN OrderDetail AS od ON od.OrderId = o.Id
                                    INNER JOIN Item AS i ON i.Id = od.ItemId
                                    WHERE CONVERT(varchar, o.OrderDateTime , 112)= convert(varchar, GETDATE(), 112)
                                    GROUP BY od.ItemId,i.Code, Name
                                    ORDER BY SUM(Quantity) DESC";

            string monthHotItemQuery = $@"SELECT top 1 od.ItemId Id,i.Code, i.Name, SUM(od.Quantity) Quantity
                                    FROM [ORDER] o INNER JOIN OrderDetail AS od ON od.OrderId = o.Id
                                    INNER JOIN Item AS i ON i.Id = od.ItemId
                                    WHERE MONTH(o.OrderDateTime) = MONTH(GETDATE()) AND YEAR(o.OrderDateTime) = YEAR(GETDATE())
                                    GROUP BY od.ItemId,i.Code, NAME
                                    ORDER BY SUM(Quantity) DESC";

            string salesDetailQuery = $@" SELECT o.Code OrderNumber,ISNULL(i.InvoiceNumber,0) InvoiceNumber , o.OrderDateTime,o.TableNumber,
                                           CASE ISNULL(i.OrderId, 0)
                                                WHEN 0 THEN s.Name
                                                ELSE 'COMPLETED'
                                           END                            OrderStatus,
                                           SUM(ISNULL(o.GrandTotal, 0)) Total,
                                           SUM(ISNULL(o.GST, 0))          GST,
                                           SUM(ISNULL(o.NetTotal, 0))     NetTotal
                                            FROM   [Order]  AS o LEFT JOIN OrderDetail       AS od
                                                    ON  od.OrderId = o.Id LEFT JOIN [Status]          AS s
                                                    ON  s.Id = o.StatusId LEFT JOIN Waiter            AS w
                                                    ON  o.WaiterId = w.Id LEFT JOIN Invoice           AS i
                                                    ON  i.OrderId = o.Id
                                            WHERE  CONVERT(VARCHAR, o.OrderDateTime, 112) = CONVERT(VARCHAR, GETDATE(), 112)
                                            GROUP BY o.Code,i.InvoiceNumber, o.OrderDateTime, o.TableNumber, w.Name, s.Name,i.OrderId";
            var monthlySales = db.Database.SqlQuery<MonthlySales>(monthlySalesQuery).ToList();
            var dailySales = db.Database.SqlQuery<TodaysSales>(dailySalesQuery).ToList();
            var todayHotItem = db.Database.SqlQuery<TodaysHotItem>(todayHotItemQuery).ToList();
            var monthHotItem= db.Database.SqlQuery<MonthHotItem>(monthHotItemQuery).ToList();
            var todaysSalesDetail = db.Database.SqlQuery<TodaysSalesDetail>(salesDetailQuery).ToList();

            if (monthlySales != null && monthlySales.Any())
            {
                dashboard.MonthlySales = monthlySales.FirstOrDefault();
            }
            else {
                dashboard.MonthlySales = new MonthlySales();
            }
            if (dailySales != null && dailySales.Any())
            {
                dashboard.TodaysSales = dailySales.FirstOrDefault();
            }
            else
            {
                dashboard.TodaysSales = new TodaysSales();
            }
            if (todayHotItem != null && todayHotItem.Any())
            {
                dashboard.TodaysHotItem = todayHotItem.FirstOrDefault();
            }
            else
            {
                dashboard.TodaysHotItem = new TodaysHotItem();
            }
            if (monthHotItem != null && monthHotItem.Any())
            {
                dashboard.MonthHotItem = monthHotItem.FirstOrDefault();
            }
            else
            {
                dashboard.MonthHotItem = new MonthHotItem();
            }
            dashboard.TodaysSalesDetail = todaysSalesDetail.ToList();
            return View(dashboard);
        }
        [HttpPost]
        [AllowAnonymous]
        public OrderViewModel SalesByDateReport(OrderViewModel data)
        {
            return null;
        }
    }
}