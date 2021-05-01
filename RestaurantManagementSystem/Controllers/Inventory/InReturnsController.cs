using Microsoft.AspNet.Identity;
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

namespace RestaurantManagementSystem.Controllers.Inventory
{
    public class InvReturnsController : Controller
    {
        private POSEntities db = new POSEntities();

        // GET: InvReturns
        public ActionResult Index()
        {
            var list = db.Database.SqlQuery<InvReturnsDto>(@"SELECT ir.Id, ir.ReturnNo, 
                 ir.ReturnDate AS ReturnDateTime, 
                ir.ReturnBy, ir.ReturnTo, ir.Description, AspNetUsers.Email AS RetunByEmail, 
                AspNetUsers_1.Email AS RetunToEmail
                FROM InvReturn AS ir LEFT OUTER JOIN
                AspNetUsers AS AspNetUsers_1 ON ir.ReturnTo = AspNetUsers_1.Id LEFT OUTER JOIN
                AspNetUsers ON ir.ReturnBy = AspNetUsers.Id");
            return View(list);
        }

        // GET: InvReturns/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvReturn invReturn = db.InvReturns.Find(id);
            InvReturnsDto returnobject = new InvReturnsDto()
            {
                Description = invReturn.Description,
                Id = invReturn.Id,
                ReturnBy = invReturn.ReturnBy,
                ReturnDateTime = invReturn.ReturnDate,
                ReturnNo = invReturn.ReturnNo,
                ReturnTo = invReturn.ReturnTo
            };

            if (invReturn == null)
            {
                return HttpNotFound();
            }
            return View(returnobject);
        }

        // GET: InvReturns/Create
        public ActionResult Create()
        {

            InvReturnsDto returnobject = new InvReturnsDto()
            {
                ReturnDateTime = DateTime.Now,
                ReturnTo = Convert.ToInt64(User.Identity.GetUserId()),
                objectState = (int)ObjectState.Added,
                listUser = db.AspNetUsers.Select(x => new UserDto() { Email = x.Email, Id = x.Id }).ToList(),
                //listItems = db.InventoryItems.Select(x => new ItemDto() { Name = x.Name, Id = x.Id }).ToList(),
                listReturnDetails = new List<InvReturnsDetailDto>()
            };

            return View(returnobject);
        }
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvReturn invReturn = db.InvReturns.Find(id);
            InvReturnsDto returnobject = new InvReturnsDto()
            {
                Description = invReturn.Description,
                Id = invReturn.Id,
                ReturnBy = invReturn.ReturnBy,
                ReturnDateTime = invReturn.ReturnDate,
                ReturnNo = invReturn.ReturnNo,
                ReturnTo = invReturn.ReturnTo,
                objectState = (int)ObjectState.Unchanged,
                listUser = db.AspNetUsers.Select(x => new UserDto() { Email = x.Email, Id = x.Id }).ToList(),
                listReturnDetails = GetDetailListing(invReturn.Id)
            };
            if (invReturn == null)
            {
                return HttpNotFound();
            }
            return View(returnobject);
        }

        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvReturn invReturn = db.InvReturns.Find(id);
            InvReturnsDto returnobject = new InvReturnsDto()
            {
                Description = invReturn.Description,
                Id = invReturn.Id,
                ReturnBy = invReturn.ReturnBy,
                ReturnDateTime = invReturn.ReturnDate,
                ReturnNo = invReturn.ReturnNo,
                ReturnTo = invReturn.ReturnTo,
                objectState = (int)ObjectState.Deleted,
                listUser = db.AspNetUsers.Select(x => new UserDto() { Email = x.Email, Id = x.Id }).ToList(),
                listReturnDetails = GetDetailListing(invReturn.Id)
            };

            if (invReturn == null)
            {
                return HttpNotFound();
            }
            return View(returnobject);
        }
        public List<InvReturnsDetailDto> GetDetailListing(long Id)
        {
            var listdetails = db.Database.SqlQuery<InvReturnsDetailDto>(@" SELECT  ird.Id, ird.InvReturnId, ird.InventoryItemId, InvReturn.ReturnNo, 
            ird.Quantity, InventoryItem.Name AS ItemName, InvReturn.Description FROM InvReturnDetail AS ird INNER JOIN
            InventoryItem ON ird.InventoryItemId = InventoryItem.Id INNER JOIN
            InvReturn ON ird.InvReturnId = InvReturn.Id WHERE ird.InvReturnId =" + Id).ToList();
            foreach (var item in listdetails)
            {
                item.listItems = db.InventoryItems.Select(x => new ItemDto() { Name = x.Name, Id = x.Id }).ToList();
            }
            return listdetails;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public async Task<JsonResult> SaveAsync(InvReturnsDto vm)
        {
            try
            {
                if (vm.listReturnDetailsToDelete.Count() > 0)
                {
                    foreach (var item in vm.listReturnDetailsToDelete)
                    {
                        var objdelete = db.InvReturnDetails.Find(item);
                        if (objdelete != null)
                        {
                            db.Entry(objdelete).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                        }
                    }
                }
                var objsave = new InvReturn()
                {
                    Id = vm.Id,
                    Description = vm.Description,
                    ReturnBy = vm.ReturnBy,
                    ReturnTo = vm.ReturnTo,
                    ReturnDate = vm.ReturnDateTime,
                    ReturnNo = vm.ReturnNo,
                };
                if (objsave.Id > 0 && vm.objectState != (int)ObjectState.Deleted)
                {
                    vm.objectState = (int)ObjectState.Modified;
                }
                if (objsave.Id <= 0 && vm.objectState != (int)ObjectState.Deleted)
                {
                    vm.objectState = (int)ObjectState.Added;
                }
                switch (vm.objectState)
                {
                    case (int)ObjectState.Added:
                        db.InvReturns.Add(objsave);
                        await db.SaveChangesAsync();
                        break;
                    case (int)ObjectState.Modified:
                        db.Entry(objsave).State = EntityState.Modified;
                        await db.SaveChangesAsync();
                        break;
                    case (int)ObjectState.Deleted:
                        var returndetails = db.InvReturnDetails.Where(x => x.InvReturnId == objsave.Id).ToList();
                        foreach (var item in returndetails)
                        {
                            var invReturndetail = await db.InvReturnDetails.FindAsync(item.Id);
                            db.Entry(invReturndetail).State = EntityState.Deleted;
                            db.SaveChanges();
                        }
                        db.Entry(objsave).State = EntityState.Deleted;
                        await db.SaveChangesAsync();
                        break;
                }
                vm.Id = objsave.Id;
                vm.listReturnDetails = await SaveDetailAsync(vm);
                var result = new { result = vm };
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        public async Task<List<InvReturnsDetailDto>> SaveDetailAsync(InvReturnsDto vm)
        {
            try
            {
                foreach (var item in vm.listReturnDetails)
                {
                    var objsave = new InvReturnDetail()
                    {
                        Id = item.Id,
                        InventoryItemId = item.InventoryItemId,
                        InvReturnId = vm.Id,
                        Quantity = item.Quantity
                    };
                    if (objsave.Id > 0 && item.objectState != (int)ObjectState.Deleted)
                    {
                        item.objectState = (int)ObjectState.Modified;
                    }
                    if (objsave.Id <= 0 && item.objectState != (int)ObjectState.Deleted)
                    {
                        item.objectState = (int)ObjectState.Added;
                    }
                    switch (item.objectState)
                    {
                        case (int)ObjectState.Added:
                            db.InvReturnDetails.Add(objsave);
                            await db.SaveChangesAsync();
                            break;
                        case (int)ObjectState.Modified:
                            db.Entry(objsave).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            break;
                    }
                    item.Id = objsave.Id;
                }
                vm.listReturnDetails = GetDetailListing(vm.Id);
                return vm.listReturnDetails;
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
