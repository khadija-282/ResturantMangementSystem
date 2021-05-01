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
using RestaurantManagementSystem.ViewModels;
using AutoMapper;
using System.Data.SqlClient;
using System.Web.Helpers;
using RestaurantManagementSystem.Enums;

namespace RestaurantManagementSystem.Controllers
{
    [CustomAuthorize("Order")]
    public class OrderController : Controller
    {
        private POSEntities db = new POSEntities();

        // GET: Order
        public async Task<ActionResult> Index()
        {
            //PRALService.ServiceSoapClient client = new PRALService.ServiceSoapClient();
            var orders = db.Database.SqlQuery<OrderViewModel>(
                            @"SELECT o.Id, o.Code, o.OrderDateTime, o.CustomerId, o.CustomerName, o.TableNumber,o.Shift,o.Location, w.Name Waiter, 
                               s.Name OrderStatus,
                            Sum(ISnull(od.LineTotal,0)) BillAmount, ISNULL(o.BillNumber,0) BillNumber
                            FROM   [Order] AS o       
                            LEFT JOIN OrderDetail AS od ON  od.OrderId = o.Id       
                            LEFT JOIN [Status]  AS s ON  s.Id = o.StatusId
                            LEFT JOIN Waiter    AS w ON  o.WaiterId = w.Id
                            LEFT JOIN SCWTimer AS i ON i.OrderId = o.Id
                            WHERE  o.StatusId NOT IN (5,6,7) 
                            GROUP BY o.Id, o.Code, o.OrderDateTime, o.CustomerId, o.CustomerName, o.TableNumber,o.Shift,o.Location, w.Name, s.Name,i.OrderId,o.BillNumber 
                            ORDER BY o.BillNumber");
            return View(orders);
        }
        public async Task<ActionResult> CompletedOrders()
        {
            //PRALService.ServiceSoapClient client = new PRALService.ServiceSoapClient();
            var completedInvoicesDays  = System.Configuration.ConfigurationManager.AppSettings["CompletedInvoicesDays"];
            var orders = db.Database.SqlQuery<OrderViewModel>(
                            $@"SELECT o.Id,o.BillNumber,o.Shift, Cast(i.InvoiceNumber as nvarchar) Code, o.OrderDateTime, o.CustomerId, o.CustomerName, o.TableNumber, w.Name Waiter, 
                               ISNULL(s.Name,'') OrderStatus,
                            Sum(ISnull(od.LineTotal,0)) BillAmount
                            FROM   [Order] AS o       
                            LEFT JOIN OrderDetail AS od ON  od.OrderId = o.Id       
                            LEFT JOIN [Status]  AS s ON  s.Id = o.StatusId
                            LEFT JOIN Waiter    AS w ON  o.WaiterId = w.Id
                            LEFT JOIN Invoice AS i ON i.OrderId = o.Id
                            WHERE  CONVERT(VARCHAR(8), o.OrderDateTime, 112) >= CONVERT(VARCHAR(8), GETDATE()-{completedInvoicesDays}, 112) AND i.OrderId IS NOT NULL AND o.StatusId={(long)EOrderStatus.COMPLETED} 
                            GROUP BY o.Id,o.BillNumber,o.Shift, o.Code, o.OrderDateTime, o.CustomerId, o.CustomerName, o.TableNumber, w.Name, s.Name,i.OrderId,i.InvoiceNumber ORDER BY o.Shift,o.OrderDateTime DESC");
            return View(orders);
        }
        public async Task<ActionResult> CancelledOrders()
        {
            //PRALService.ServiceSoapClient client = new PRALService.ServiceSoapClient();
            var orders = db.Database.SqlQuery<OrderViewModel>(
                            @"SELECT o.Id, o.Code, o.OrderDateTime, o.CustomerId, o.CustomerName, o.TableNumber, w.Name Waiter, 
                               CASE ISNULL(i.OrderId,0) WHEN 0 THEN s.Name ELSE 'COMPLETED' END OrderStatus,
                            Sum(ISnull(od.LineTotal,0)) BillAmount
                            FROM   [Order] AS o       
                            LEFT JOIN OrderDetail AS od ON  od.OrderId = o.Id       
                            LEFT JOIN [Status]  AS s ON  s.Id = o.StatusId
                            LEFT JOIN Waiter    AS w ON  o.WaiterId = w.Id
                            LEFT JOIN Invoice AS i ON i.OrderId = o.Id
                            WHERE  CONVERT(VARCHAR(8), o.OrderDateTime, 112) = CONVERT(VARCHAR(8), GETDATE(), 112) AND o.StatusId=2
                            GROUP BY o.Id, o.Code, o.OrderDateTime, o.CustomerId, o.CustomerName, o.TableNumber, w.Name, s.Name,i.OrderId");
            return View(orders);
        }

        // GET: Order/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Order/Create
        public ActionResult Create()
        {
            ViewBag.Locations = new SelectList(db.Locations.Select(t => new { Id = t.Code, Name = t.Code}), "Id", "Name");
            ViewBag.Customers = new SelectList(db.Customers.Select(t => new { Id = t.Id, Name = t.FirstName }), "Id", "Name");
            ViewBag.Status = new SelectList(db.Status.Select(t => new { Id = t.Id, Name = t.Name }), "Id", "Name");
            ViewBag.ItemsJson = db.Items.Select(t => new { Id = t.Id, Code = t.Code, Name = t.Name, Description = t.Description, CategoryId = t.CategoryId, ImageURL = t.ImageURL, Price = t.Price, VAT = t.VAT, DiscountType = t.DiscountType, DiscountValue = t.DiscountValue, ValidFromDate = t.ValidFromDate, ValidToDate = t.ValidToDate, IsDealItem_YN = t.IsDealItem_YN }).ToList();
            //ViewBag.Items = db.Items;
            var allitems = db.Items.ToList();
            ViewBag.Items = allitems;
            ViewBag.FastFoods = allitems.Where(t => t.CategoryId == 1);
            ViewBag.Drinks = allitems.Where(t => t.CategoryId == 2);
            ViewBag.Salads = allitems.Where(t => t.CategoryId == 3);
            ViewBag.General = allitems.Where(t => t.CategoryId == 4);
            ViewBag.Chinese = allitems.Where(t => t.CategoryId == 5);
            ViewBag.Pakistani = allitems.Where(t => t.CategoryId == 6);
            ViewBag.SeaFoods = allitems.Where(t => t.CategoryId == 7);
            ViewBag.Categories = db.Categories;
            ViewBag.Waiters = new SelectList(db.Waiters.Select(t => new { Id = t.Id, Name = t.Name }), "Id", "Name");
            OrderViewModel vm = new OrderViewModel();
            vm.OrderDateTime = DateTime.Now;
            return View(vm);
        }

        // POST: Order/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderViewModel newOrder)
        {
            DateTime orderDatetime = newOrder.OrderDateTime.HasValue ? newOrder.OrderDateTime.Value : DateTime.Now;
            DbContextTransaction transaction = null;
            try
            {
                if (ModelState.IsValid)
                {
                    var ShiftAStart = TimeSpan.Parse(System.Configuration.ConfigurationManager.AppSettings["ShiftAStart"]);
                    var ShiftAEnd = TimeSpan.Parse(System.Configuration.ConfigurationManager.AppSettings["ShiftAEnd"]);
                    var ShiftBStart = TimeSpan.Parse(System.Configuration.ConfigurationManager.AppSettings["ShiftBStart"]);
                    var ShiftBEnd = TimeSpan.Parse(System.Configuration.ConfigurationManager.AppSettings["ShiftBEnd"]);
                    string shift = "A";
                    if (ShiftAStart>=DateTime.Now.TimeOfDay  && ShiftAEnd<= DateTime.Now.TimeOfDay)
                    {
                        shift = "A";
                    }
                    else //if (ShiftBStart >= DateTime.Now.TimeOfDay && ShiftBEnd<= DateTime.Now.TimeOfDay)
                    {
                        shift = "B";
                    }
                    transaction = db.Database.BeginTransaction();
                    var order = Mapper.Map<Order>(newOrder);
                    if (order.Id == 0)
                    {
                        order.OrderDateTime = orderDatetime;
                        order.Shift = shift;
                        order.Code = await NextOrderNumber();
                        order.StatusId = (int)EOrderStatus.NEW;
                        db.Orders.Add(order);
                    }
                    else
                    {
                        if (db.Invoices.Where(t => t.OrderId == newOrder.Id).Count() > 0)
                        {
                            if (db.Invoices.Where(t => t.OrderId == newOrder.Id).Count() > 0)
                            {
                                return Json(new { success = true, responseText = "Order cannot be edited. Invoice has been generated for this order." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        order.OrderDateTime = orderDatetime;
                        order.StatusId = (int)EOrderStatus.NEW;
                        order.UpdatedOn = DateTime.Now;
                        var delayedInvoice = db.SCWTimers.Where(t => t.OrderId == newOrder.Id).FirstOrDefault();
                        if (delayedInvoice != null)
                        {
                            db.SCWTimers.Remove(delayedInvoice);
                        }
                        order.Shift = shift;
                        db.Entry(order).State = EntityState.Modified;
                        var deleteStatus = db.Database.ExecuteSqlCommand($"DELETE FROM OrderDetail WHERE OrderId={order.Id}");
                        newOrder.OrderDetail.ForEach(t => t.Id = null);
                    }

                    var orderId = await db.SaveChangesAsync();
                    newOrder.OrderDetail.ForEach(t => t.OrderId = order.Id);

                    foreach (var item in newOrder.OrderDetail)
                    {
                        var orderDetail = Mapper.Map<OrderDetail>(item);
                        orderDetail.Item = null;
                        db.OrderDetails.Add(orderDetail);
                    }
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return Json(new { Id = order.Id, Code = order.Code, success = true, responseText = "Order saved succesfuly." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(new { success = false, responseText = $"{ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, responseText = "Please review selected item." }, JsonRequestBehavior.AllowGet);
        }

        // GET: Order/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            var orderDetail = db.Database.SqlQuery<OrderDetailViewModel>($@"SELECT od.Id, od.OrderId,od.ItemId,i.Name ItemName,i.Price,od.Quantity,od.LineTotal,i.VAT,i.DiscountType,i.DiscountValue,0.0 GrandTotal
                                                                            FROM OrderDetail AS od INNER JOIN Item AS i ON i.Id = od.ItemId WHERE od.OrderId={id}");
            var allitems = db.Items;
            ViewBag.Items = allitems.ToList();
            ViewBag.FastFoods = allitems.Where(t => t.CategoryId == 1);
            ViewBag.Drinks = allitems.Where(t => t.CategoryId == 2);
            ViewBag.Salads = allitems.Where(t => t.CategoryId == 3);
            ViewBag.General = allitems.Where(t => t.CategoryId == 4);
            ViewBag.Chinese = allitems.Where(t => t.CategoryId == 5);
            ViewBag.Pakistani = allitems.Where(t => t.CategoryId == 6);
            ViewBag.SeaFoods = allitems.Where(t => t.CategoryId == 7);
            ViewBag.Categories = db.Categories;

            ViewBag.Locations = new SelectList(db.Locations.Select(t => new { Id = t.Code, Name = t.Code }), "Id", "Name");
            ViewBag.Customers = new SelectList(db.Customers.Select(t => new { Id = t.Id, Name = t.FirstName }), "Id", "Name");
            ViewBag.Status = new SelectList(db.Status.Select(t => new { Id = t.Id, Name = t.Name }), "Id", "Name");
            ViewBag.Waiters = new SelectList(db.Waiters.Select(t => new { Id = t.Id, Name = t.Name }), "Id", "Name");

            OrderViewModel vm = new OrderViewModel();
            vm.Id = order.Id;
            vm.Code = order.Code;
            vm.Description = order.Description;
            vm.OrderDateTime = order.OrderDateTime;
            vm.ExpectedServingTime = order.ExpectedServingTime;
            vm.CustomerId = order.CustomerId;
            vm.CustomerName = order.CustomerName;
            vm.CreatedDate = order.CreatedDate;
            vm.CreatedBy = order.CreatedBy;
            vm.StatusId = order.StatusId;
            vm.Location = order.Location;
            vm.Shift = order.Shift;
            vm.TableNumber = order.TableNumber;
            vm.Pax = order.Pax;
            vm.BillNumber = order.BillNumber;
            vm.WaiterId = order.WaiterId;
            vm.GrandTotal = order.GrandTotal;
            vm.DiscountValue = order.DiscountValue;
            vm.GST = order.GST;
            vm.NetTotal = order.NetTotal;
            vm.BillNumber = order.BillNumber;
            vm.OrderDetail = orderDetail.ToList();
            ViewBag.Order = Json(vm, JsonRequestBehavior.AllowGet);
            return View(vm);
        }

        public async Task<JsonResult> GetInvoice(long? id)
        {
            var defaultServiceCharges = System.Configuration.ConfigurationManager.AppSettings["DefaultServiceCharges"];
            var defaultDiscount= System.Configuration.ConfigurationManager.AppSettings["DefaultDiscount"];
            if (id == null)
            {
                return Json(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            //GET NEXT INVOICE NUMBER and UPDATE ORDER Number.
            if (order == null)
            {
                return Json(HttpNotFound());
            }
            var orderDetail = db.Database.SqlQuery<OrderDetailViewModel>($@"SELECT od.Id,
                                                                           od.OrderId,
                                                                           od.ItemId,
                                                                           i.Name                     ItemName,
                                                                           i.Price,
                                                                           od.Quantity,
                                                                           od.LineTotal,
                                                                           i.VAT,
                                                                           i.DiscountType,
                                                                           i.DiscountValue,
                                                                           0.0                        GrandTotal,
                                                                           o.TableNumber
                                                                           FROM   [Order]                 AS o
                                                                           INNER JOIN OrderDetail  AS od
                                                                                ON  od.OrderId = o.Id
                                                                           INNER JOIN Item         AS i
                                                                                ON  i.Id = od.ItemId  WHERE od.OrderId={id}");
            var invoice = db.SCWTimers.Where(t => t.OrderId == order.Id).FirstOrDefault();
            OrderViewModel vm = new OrderViewModel();
            vm.Id = order.Id;
            if (invoice != null)
            {
                vm.IsNewOrder = false;
                vm.Code = Convert.ToString(invoice.InvoiceNumber);
                vm.DiscountValue = invoice.DiscountValue;
                vm.ServiceCharges = invoice.ServiceCharges;
                vm.NetTotal = invoice.NetTotal;
            }
            else
            {
                vm.IsNewOrder = true;
                vm.Code = await NextInvoiceNumber();
                vm.DiscountValue = Convert.ToDecimal(defaultDiscount);
                vm.ServiceCharges = Convert.ToDecimal(defaultServiceCharges);
                vm.NetTotal = order.NetTotal;
            }
            vm.Description = order.Description;
            vm.OrderDateTime = order.OrderDateTime.Value;
            vm.ExpectedServingTime = order.ExpectedServingTime;
            vm.CustomerId = order.CustomerId;
            if (order.CustomerId == 1)
                vm.CustomerName = "Dine-In";
            else if (order.CustomerId == 2)
                vm.CustomerName = "Take Away";
            else
                vm.CustomerName = "Delivery";

            vm.CreatedDate = order.CreatedDate;
            vm.CreatedBy = order.CreatedBy;
            vm.StatusId = order.StatusId;
            vm.TableNumber = order.TableNumber;
            vm.GrandTotal = order.GrandTotal;
            vm.GST = order.GST;
            vm.Pax = order.Pax;
            vm.BillNumber = order.BillNumber;
            vm.WaiterId = order.WaiterId;
            vm.OrderDetail = orderDetail.ToList();
            return Json(vm, JsonRequestBehavior.AllowGet);
        }
        // POST: Order/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderViewModel newOrder)
        {
            DbContextTransaction transaction = null;
            try
            {
                if (ModelState.IsValid)
                {
                    transaction = db.Database.BeginTransaction();
                    var order = Mapper.Map<Order>(newOrder);
                    if (order.Id == 0)
                    {
                        db.Orders.Add(order);
                    }
                    else
                    {
                        if (db.Invoices.Where(t => t.OrderId == newOrder.Id).Count() > 0)
                        {
                            if (db.Invoices.Where(t => t.OrderId == newOrder.Id).Count() > 0)
                            {
                                return Json(new { success = true, responseText = "Order cannot be edited. Invoice has been generated for this order." }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        var delayedInvoice = db.SCWTimers.Where(t => t.OrderId == newOrder.Id).FirstOrDefault();
                        if (delayedInvoice != null)
                        {
                            db.SCWTimers.Remove(delayedInvoice);
                        }
                        db.Entry(order).State = EntityState.Modified;
                    }
                    order.Code = await NextOrderNumber();
                    var orderId = await db.SaveChangesAsync();
                    newOrder.OrderDetail.ForEach(t => t.OrderId = order.Id);
                    string detailIds = "";
                    foreach (var item in newOrder.OrderDetail)
                    {
                        if (item.Id.HasValue)
                            detailIds = $"{detailIds}{item.Id},";
                    }
                    var deleteStatus = 0;
                    if (!String.IsNullOrWhiteSpace(detailIds))
                        deleteStatus = db.Database.ExecuteSqlCommand($"DELETE FROM OrderDetail WHERE Id IN ({detailIds})");
                    foreach (var item in newOrder.OrderDetail)
                    {
                        var orderDetail = Mapper.Map<OrderDetail>(item);
                        if (item.Id.HasValue && item.Id.Value > 0)
                        {
                            db.OrderDetails.Remove(orderDetail);
                            orderDetail.Id = 0;
                        }
                        orderDetail.Item = null;
                        db.OrderDetails.Add(orderDetail);
                    }
                    await db.SaveChangesAsync();
                    transaction.Commit();
                    return Json(new { success = true, responseText = "Order saved succesfuly." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(new { success = false, responseText = $"{ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, responseText = "Please review selected item." }, JsonRequestBehavior.AllowGet);
        }

        // GET: Order/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = await db.Orders.FindAsync(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Order/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Order order = await db.Orders.FindAsync(id);
            db.Orders.Remove(order);
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
        public async Task<string> NextOrderNumber()
        {
            var NextOrderNumber = db.Database.SqlQuery<string>(@"SELECT CAST(NEXT VALUE FOR OrderNumber AS NVARCHAR)").FirstOrDefault();
            return NextOrderNumber;
        }
        public async Task<string> NextInvoiceNumber()
        {
            var nextInvoiceNumber = db.Database.SqlQuery<long>(@"SELECT ISNULL(Max(InvoiceNumber),0)+1 NextInvoiceNumber FROM SCWTimer").FirstOrDefault();
            var invoiceNumber = Convert.ToString(nextInvoiceNumber);
            return invoiceNumber;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> CancelOrder(long orderId)
        {
            if (orderId > 0)
            {
                var order = db.Orders.Find(orderId);
                if (order != null && order.StatusId != (long)EOrderStatus.COMPLETED)
                {
                    if (Common.CSNCommon.DeleteOrderInfoFromTables)
                    {
                        var invoice = db.SCWTimers.Where(t => t.OrderId == orderId);
                        if (invoice.Any() && invoice.Count() > 0)
                        {
                            var deleteInvoice = invoice.FirstOrDefault();
                            db.SCWTimers.Remove(deleteInvoice);
                        }
                        var orderDetails = db.OrderDetails.Where(o => o.OrderId == order.Id);
                        db.OrderDetails.RemoveRange((orderDetails));
                        db.Orders.Remove(order);
                    }
                    else
                    {
                        var invoice = db.SCWTimers.Where(t => t.OrderId == orderId);
                        if (invoice.Any() && invoice.Count() > 0)
                        {
                            var invoiceToUpdate = db.SCWTimers.Where(t => t.OrderId == orderId);
                            var deleteInvoice = invoiceToUpdate.FirstOrDefault();
                            deleteInvoice.IsDeleted = true;
                            db.Entry(deleteInvoice).State = EntityState.Modified;
                        }
                        order.StatusId = (int)EOrderStatus.DELELETED;
                        db.Entry(order).State = EntityState.Modified;
                    }
                    var result = await db.SaveChangesAsync();
                    return Json(new { success = true, responseText = "Order has been cancelled." }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new {success = false, responseText = "Order could not be cancelled as invoice has been generated. Please edit the order and try cancell after that." },
                JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> ProcessOrder(long orderId)
        {
            JsonResult response = null;
            try
            {
                var order = db.Orders.Find(orderId);
                var scwInvoice = db.SCWTimers.Where(t => t.OrderId == orderId);
                if (order == null && !scwInvoice.Any()) 
                {
                    response = Json(new { success = false, responseText = "Invalid order number." }, JsonRequestBehavior.AllowGet);
                    return response;
                }
                if (order.StatusId == (long)EOrderStatus.NEW)
                {
                    response = Json(new { success = false, responseText = "Order cannot be processed before Invoice." }, JsonRequestBehavior.AllowGet);
                    return response;
                }
                //UPDATE INVOICE STATUS IN SCWInvoice TABLE
                SCWTimer scwInvoiceRow = scwInvoice.FirstOrDefault();
                scwInvoiceRow.IsInvoiced = true;
                db.Entry(scwInvoiceRow).State = EntityState.Modified;

                //UPDATE INVOICE STATUS IN SCWInvoice TABLE
                order.StatusId = (long)EOrderStatus.COMPLETED;
                db.Entry(order).State = EntityState.Modified;
                
                var invoice = new Invoice()
                {
                    OrderId = scwInvoice.FirstOrDefault().OrderId,
                    InvoiceNumber = scwInvoiceRow.InvoiceNumber,
                    InvoiceDate = scwInvoiceRow.InvoiceDate,
                    GrandTotal = scwInvoiceRow.GrandTotal,
                    DiscountValue = scwInvoiceRow.DiscountValue,
                    GST = scwInvoiceRow.GST,
                    NetTotal = scwInvoiceRow.NetTotal,
                    PaymentType = scwInvoiceRow.PaymentType,
                    ServiceCharges = scwInvoiceRow.ServiceCharges,
                    IsSentToPRA = true,
                    IsDeleted = false,
                    UpdatedOn = DateTime.Now,
                };
                db.Invoices.Add(invoice);
                var processStatus = await db.SaveChangesAsync();
                if (processStatus > 0)
                {
                    response = Json(new { success = true, responseText = "Order has been completed." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                response = Json(new { success = false, responseText = "Invalid order number." }, JsonRequestBehavior.AllowGet);
                throw;
            }
            return response;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GenerateInvoice(SCWTimer invoice)
        {
            string message = "Invoice Generated.";
            bool isValidData = true;
            try
            {

                invoice.InvoiceDate = DateTime.Now;
                if (invoice.InvoiceNumber == null || invoice.InvoiceNumber <= 0)
                {
                    message = "Invoice Number is not valid.";
                    isValidData = false;
                }
                else if (invoice.OrderId == null || invoice.OrderId <= 0)
                {
                    message = "Order Not selected.";
                    isValidData = false;
                }
                else if (invoice.GrandTotal == null || invoice.GrandTotal <= 0)
                {
                    message = "Invoice amount must be greater than 0.";
                    isValidData = false;
                }
                if (isValidData)
                {
                    var checkIfAlreadyGenerated = db.SCWTimers.Where(t => t.OrderId == invoice.OrderId);
                    if (checkIfAlreadyGenerated != null && checkIfAlreadyGenerated.Any())
                    {
                        return Json(new { success = false, responseText = "Invoice Already Generated." }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (invoice.IsDeleted.GetValueOrDefault())
                        {
                            invoice.IsDeleted = true;
                            invoice.IsInvoiced = false;
                        }
                        else
                        {
                            invoice.IsDeleted = false;
                        }
                        var order = db.Orders.Where(t => t.Id == invoice.OrderId).FirstOrDefault();
                        order.StatusId = (int)EOrderStatus.PENDING;
                        db.Entry(order).State = EntityState.Modified;
                        invoice.IsInvoiced = false;
                        //invoice.UpdatedOn = DateTime.Now;
                        invoice.InvoiceDate = order.OrderDateTime.Value;
                        var invoiceData = db.SCWTimers.Add(invoice);
                        var result = await db.SaveChangesAsync();
                        try
                        {
                            if (Common.CSNCommon.DataSync && !invoice.IsDeleted.GetValueOrDefault())
                            {
                                /*PRALService.ServiceSoap client = new PRALService.ServiceSoapClient("ServiceSoap12");
                                var praUpdateStatus = client.Resturant_Details(Common.CSNCommon.RestraurantId, invoice.InvoiceDate, invoice.InvoiceNumber.ToString(), 
                                    invoice.GrandTotal, invoice.GST, (invoice.GrandTotal + invoice.GST), 0, 0, 0, "0", false);
                                if (praUpdateStatus >= 1)
                                {
                                    var updateInvoice = db.Invoices.Find(invoiceData.Id);
                                    updateInvoice.IsSentToPRA = true;
                                    var updateResult = await db.SaveChangesAsync();
                                }
                                */
                            }
                            var updateInvStatus = await UpdateInventory(invoice.OrderId);
                        }
                        catch (Exception ex)
                        {
                            throw;
                        }
                        return Json(new { success = true, responseText = $"{message}" }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { success = false, responseText = $"{message}" }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = $"{ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> UpdateInventory(long OrderId)
        {
            try
            {
                int updateStatus = 0;
                var order = db.Orders.Include(t => t.OrderDetails).Where(t => t.Id == OrderId);
                if (order.Any())
                {
                    var orderDetails = order.FirstOrDefault().OrderDetails;
                    foreach (var item in orderDetails)
                    {
                        var recipe = db.Recipes.Where(t => t.MenuItemId == item.ItemId).ToList();
                        foreach (var menuItem in recipe)
                        {
                            var InventoryItem = db.InventoryItems.Find(menuItem.MenuItemId);
                            InventoryItem.TotalPackages = InventoryItem.TotalPackages.GetValueOrDefault() - (double)((double)menuItem.Quantity * (double)item.Quantity);
                            updateStatus = db.SaveChanges();
                        }
                    }
                }
                if (updateStatus > 0)
                {
                    return Json(new { success = true, responseText = "Inventory Item balance have been updated." }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, responseText = "Inventory could not be update." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, responseText = $"{ex.Message}" }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}