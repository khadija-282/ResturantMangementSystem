using RestaurantManagementSystem.Enums;
using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace RestaurantManagementSystem.Controllers
{
    public class InvReceiptsController : Controller
    {
        private POSEntities db = new POSEntities();

        // GET: InvReceipts
        public ActionResult Index()
        {
            var list = db.Database.SqlQuery<InvReceiptDto>(
                           @"SELECT      InvReceipt.Id, InvReceipt.ReceiptNo, InvReceipt.ReceiptDate, ReceivedPlace,
			InvReceipt.VendorId AS Vendor, InvReceipt.ReceivedBy AS ReceivedById, AspNetUsers.Email AS ReceivedBy,  
			InvReceipt.Description, Vendor.Name AS VendorName
            from InvReceipt LEFT OUTER JOIN
                         Vendor ON InvReceipt.VendorId = Vendor.Id LEFT OUTER JOIN
                         AspNetUsers ON InvReceipt.ReceivedBy = AspNetUsers.Id").ToList();
            return View(list);
        }

        // GET: InvReceipts/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvReceipt invReceipt = db.InvReceipts.Find(id);
            InvReceiptDto invreceiptdto = new InvReceiptDto()
            {
                Description = invReceipt.Description,
                Id = invReceipt.Id,
                ObjectState = (int)ObjectState.Unchanged,
                ReceiptDate = invReceipt.ReceiptDate,
                ReceiptNo = invReceipt.ReceiptNo,
                ReceivedById = invReceipt.ReceivedBy,
                VendorId = invReceipt.VendorId,
                listUser = db.AspNetUsers.Select(x => new UserDto { Email = x.Email, Id = x.Id }).ToList(),
                listVendor = db.Vendors.Select(x => new VendorDto() { Id = x.Id, Name = x.Name }).ToList(),
                listDetail = GetDetails(invReceipt.Id),
                listReceivedPlace = RecievedPlaceDto.ConvertEnum(),
                ReceivedPlaceId= (int)Enum.Parse(typeof(RecievePlace), invReceipt.ReceivedPlace)
            };
            if (invReceipt == null)
            {
                return HttpNotFound();
            }
            return View(invreceiptdto);
        }
        public List<InvReceiptDetailDto> GetDetails(long Id)
        {
            var list = new List<InvReceiptDetailDto>();
            list = db.Database.SqlQuery<InvReceiptDetailDto>(@"SELECT InvReceiptDetail.Id AS DetailId, InvReceiptDetail.InvReceiptId, 
                InvReceiptDetail.InventoryItemId, InvReceiptDetail.Quantity, InvReceiptDetail.Price, 
                InventoryItem.Name as ItemName FROM InvReceiptDetail INNER JOIN
                InventoryItem ON InvReceiptDetail.InventoryItemId = InventoryItem.Id
                where InvReceiptDetail.InvReceiptId= " + Id).ToList();
            foreach (var item in list)
            {
                item.listItems = db.InventoryItems.Select(x => new ItemDto { Name = x.Name, Id = x.Id }).ToList();
            }
            return list;
        }
        // GET: InvReceipts/Create
        public ActionResult Create()
        {
            InvReceiptDto invreceiptdto = new InvReceiptDto()
            {
                ObjectState = (int)ObjectState.Added,
                ReceiptDate = DateTime.Now,
                listUser = db.AspNetUsers.Select(x => new UserDto { Email = x.Email, Id = x.Id }).ToList(),
                listVendor = db.Vendors.Select(x => new VendorDto() { Id = x.Id, Name = x.Name }).ToList(),
                listReceivedPlace = RecievedPlaceDto.ConvertEnum()
            };
            return View(invreceiptdto);
        }

        // POST: InvReceipts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.

        // GET: InvReceipts/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvReceipt invReceipt = db.InvReceipts.Find(id);
            InvReceiptDto invreceiptdto = new InvReceiptDto()
            {
                Description = invReceipt.Description,
                Id = invReceipt.Id,
                ObjectState = (int)ObjectState.Unchanged,
                ReceiptDate = invReceipt.ReceiptDate,
                ReceiptNo = invReceipt.ReceiptNo,
                ReceivedById = invReceipt.ReceivedBy,
                VendorId = invReceipt.VendorId,
                listUser = db.AspNetUsers.Select(x => new UserDto { Email = x.Email, Id = x.Id }).ToList(),
                listVendor = db.Vendors.Select(x => new VendorDto() { Id = x.Id, Name = x.Name }).ToList(),
                listDetail = GetDetails(invReceipt.Id),
                listReceivedPlace = RecievedPlaceDto.ConvertEnum(),
                ReceivedPlaceId = (int)Enum.Parse(typeof(RecievePlace), invReceipt.ReceivedPlace)
            };
            if (invReceipt == null)
            {
                return HttpNotFound();
            }
            return View(invreceiptdto);
        }


        // GET: InvReceipts/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvReceipt invReceipt = db.InvReceipts.Find(id);
            InvReceiptDto invreceiptdto = new InvReceiptDto()
            {
                Description = invReceipt.Description,
                Id = invReceipt.Id,
                ObjectState = (int)ObjectState.Deleted,
                ReceiptDate = invReceipt.ReceiptDate,
                ReceiptNo = invReceipt.ReceiptNo,
                ReceivedById = invReceipt.ReceivedBy,
                VendorId = invReceipt.VendorId,
                listUser = db.AspNetUsers.Select(x => new UserDto { Email = x.Email, Id = x.Id }).ToList(),
                listVendor = db.Vendors.Select(x => new VendorDto() { Id = x.Id, Name = x.Name }).ToList(),
                listDetail = GetDetails(invReceipt.Id),
                listReceivedPlace = RecievedPlaceDto.ConvertEnum(),
                ReceivedPlaceId = (int)Enum.Parse(typeof(RecievePlace), invReceipt.ReceivedPlace)
            };
            if (invReceipt == null)
            {
                return HttpNotFound();
            }
            return View(invreceiptdto);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public async Task<JsonResult> SaveAsync(InvReceiptDto vm)
        {
            try
            {
                if (vm.listDetailToDelete.Count() > 0)
                {
                    foreach (var item in vm.listDetailToDelete)
                    {
                        var objdelete = db.InvReceiptDetails.Find(item);
                        if (objdelete != null)
                        {
                            db.Entry(objdelete).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                    }
                }
                var objsave = new InvReceipt()
                {
                    Id = vm.Id,
                    Description = vm.Description,
                    ReceiptDate = vm.ReceiptDate,
                    ReceiptNo = vm.ReceiptNo,
                    ReceivedBy = vm.ReceivedById,
                    VendorId = vm.VendorId,
                    ReceivedPlace= ((RecievePlace)vm.ReceivedPlaceId).ToString()
                };
                if (objsave.Id > 0 && vm.ObjectState != (int)ObjectState.Deleted)
                {
                    vm.ObjectState = (int)ObjectState.Modified;
                }
                if (objsave.Id <= 0 && vm.ObjectState != (int)ObjectState.Deleted)
                {
                    vm.ObjectState = (int)ObjectState.Added;
                }
                switch (vm.ObjectState)
                {
                    case (int)ObjectState.Added:
                        db.InvReceipts.Add(objsave);
                        await db.SaveChangesAsync();
                        break;
                    case (int)ObjectState.Modified:
                        db.Entry(objsave).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    case (int)ObjectState.Deleted:
                        var returndetails = db.InvReceiptDetails.Where(x => x.InvReceiptId == objsave.Id).ToList();
                        foreach (var item in returndetails)
                        {
                            var invReturndetail = await db.InvReceiptDetails.FindAsync(item.Id);
                            db.Entry(invReturndetail).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                        db.Entry(objsave).State = EntityState.Deleted;
                        await db.SaveChangesAsync();
                        break;
                }
                vm.Id = objsave.Id;
                vm.listDetail = await SaveDetailAsync(vm);
                var result = new { result = vm };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        public async Task<List<InvReceiptDetailDto>> SaveDetailAsync(InvReceiptDto vm)
        {
            try
            {
                foreach (var item in vm.listDetail)
                {
                    var objsave = new InvReceiptDetail()
                    {
                        Id = item.DetailId,
                        InventoryItemId = item.InventoryItemId,
                        InvReceiptId = vm.Id,
                        Quantity = item.Quantity
                    };
                    if (objsave.Id > 0 && item.ObjectState != (int)ObjectState.Deleted)
                    {
                        item.ObjectState = (int)ObjectState.Modified;
                    }
                    if (objsave.Id <= 0 && item.ObjectState != (int)ObjectState.Deleted)
                    {
                        item.ObjectState = (int)ObjectState.Added;
                    }
                    switch (item.ObjectState)
                    {
                        case (int)ObjectState.Added:
                            db.InvReceiptDetails.Add(objsave);
                            await db.SaveChangesAsync();
                            break;
                        case (int)ObjectState.Modified:
                            db.Entry(objsave).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            break;
                    }
                    item.DetailId = objsave.Id;
                }
                vm.listDetail = GetDetails(vm.Id);
                return vm.listDetail;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<JsonResult> ListItems()
        {
            try
            {
                var list = db.InventoryItems.Select(x => new ItemDto() { Name = x.Name, Id = x.Id }).ToList();
                var result = new { result = list };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
    }
}
