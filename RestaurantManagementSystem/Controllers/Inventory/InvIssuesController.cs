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

namespace RestaurantManagementSystem.Controllers
{
    public class InvIssuesController : Controller
    {
        private POSEntities db = new POSEntities();

        // GET: InvIssues
        public ActionResult Index()
        {
            var list = db.Database.SqlQuery<InvIssueDto>(@"SELECT InvIssue.Id,IssuePlace,
                InvIssue.IssueNo, isnull(InvIssue.IssueDate,'') as IssueDateString, InvIssue.IssueTo, InvIssue.IssueBy, 
                InvIssue.Description, AspNetUsers_1.Email AS IssueToEmail, AspNetUsers.Email AS IssueByEmail
                FROM InvIssue INNER JOIN AspNetUsers AS AspNetUsers_1 ON InvIssue.IssueTo = AspNetUsers_1.Id INNER JOIN
                AspNetUsers ON InvIssue.IssueBy = AspNetUsers.Id").ToList();
            return View(list);
        }

        // GET: InvIssues/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvIssue invIssue = db.InvIssues.Find(id);
            InvIssueDto invIssueDto = new InvIssueDto()
            {
                Id = invIssue.Id,
                Description = invIssue.Description,
                IssueBy = invIssue.IssueBy,
                IssueDate = Convert.ToDateTime(invIssue.IssueDate),
                IssueTo = invIssue.IssueTo,
                IssueNo = invIssue.IssueNo,
                IssuePlaceId = (int)Enum.Parse(typeof(IssuePlace), invIssue.IssuePlace),
                listUser = db.AspNetUsers.Select(x => new UserDto() { Email = x.Email, Id = x.Id }).ToList(),
                ObjectState = (int)ObjectState.Unchanged,
                listIssueDetail = GetIssueDetails(invIssue.Id),
                IssuePlaceList = IssuePlaceDto.ConvertEnum()
            };
            if (invIssue == null)
            {
                return HttpNotFound();
            }
            return View(invIssueDto);
        }

        // GET: InvIssues/Create
        public ActionResult Create()
        {
            InvIssueDto invIssueDto = new InvIssueDto()
            {
                Id = 0,
                IssueDate = DateTime.Now,
                IssueBy = Convert.ToInt64(User.Identity.GetUserId()),
                IssuePlaceList = IssuePlaceDto.ConvertEnum(),
                ObjectState = (int)ObjectState.Added
            };
            invIssueDto.listUser = db.AspNetUsers.Select(x => new UserDto() { Email = x.Email, Id = x.Id }).ToList();
            return View(invIssueDto);
        }


        // GET: InvIssues/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvIssue invIssue = db.InvIssues.Find(id);
            InvIssueDto invIssueDto = new InvIssueDto()
            {
                Id = invIssue.Id,
                Description = invIssue.Description,
                IssueBy = invIssue.IssueBy,
                IssueDate = Convert.ToDateTime(invIssue.IssueDate),
                IssueTo = invIssue.IssueTo,
                IssueNo = invIssue.IssueNo,
                IssuePlaceId = (int)Enum.Parse(typeof(IssuePlace), invIssue.IssuePlace),
                listUser = db.AspNetUsers.Select(x => new UserDto() { Email = x.Email, Id = x.Id }).ToList(),
                ObjectState = (int)ObjectState.Modified,
                listIssueDetail = GetIssueDetails(invIssue.Id),
                IssuePlaceList = IssuePlaceDto.ConvertEnum()
            };
            if (invIssue == null)
            {
                return HttpNotFound();
            }
            return View(invIssueDto);
        }

        // GET: InvIssues/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InvIssue invIssue = db.InvIssues.Find(id);
            InvIssueDto invIssueDto = new InvIssueDto()
            {
                Id = invIssue.Id,
                Description = invIssue.Description,
                IssueBy = invIssue.IssueBy,
                IssueDate = Convert.ToDateTime(invIssue.IssueDate),
                IssueTo = invIssue.IssueTo,
                IssueNo = invIssue.IssueNo,
                IssuePlaceId= (int)Enum.Parse(typeof(IssuePlace), invIssue.IssuePlace),
                listUser = db.AspNetUsers.Select(x => new UserDto() { Email = x.Email, Id = x.Id }).ToList(),
                ObjectState = (int)ObjectState.Deleted,
                listIssueDetail = GetIssueDetails(invIssue.Id),
                IssuePlaceList = IssuePlaceDto.ConvertEnum()
            };
            if (invIssue == null)
            {
                return HttpNotFound();
            }
            return View(invIssueDto);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        public List<InvIssueDetailDto> GetIssueDetails(long? Id)
        {
            var list = db.Database.SqlQuery<InvIssueDetailDto>(@"SELECT iid.Id, iid.InvIssueId, iid.InventoryItemId, iid.Quantity, InvIssue.IssueNo, 
                InventoryItem.Name AS ItemName FROM InvIssueDetail AS iid INNER JOIN
                InventoryItem ON iid.InventoryItemId = InventoryItem.Id INNER JOIN InvIssue ON iid.InvIssueId = InvIssue.Id
                where iid.InvIssueId = " + Id).ToList();
            foreach (var item in list)
            {
                item.listItems = db.InventoryItems.Select(t => new ItemDto { Name = t.Name, Id = t.Id }).ToList();
                item.ObjectState = (int)ObjectState.Unchanged;
            }
            return list;
        }
        public async Task<JsonResult> SaveAsync(InvIssueDto vm)
        {
            try
            {
                if (ModelState.IsValid)
                {


                    if (vm.listIssueDetailsToDelete.Count() > 0)
                    {
                        foreach (var item in vm.listIssueDetailsToDelete)
                        {
                            var objdelete = db.InvIssueDetails.Find(item);
                            if (objdelete != null)
                            {
                                db.Entry(objdelete).State = EntityState.Deleted;
                                await db.SaveChangesAsync();
                            }
                        }
                    }
                    var objsave = new InvIssue()
                    {
                        Id = vm.Id,
                        Description = vm.Description,
                        IssueBy = vm.IssueBy,
                        IssueTo = vm.IssueTo,
                        IssueDate = vm.IssueDate.ToString(),
                        IssueNo = vm.IssueNo,
                        IssuePlace = ((IssuePlace)vm.IssuePlaceId).ToString()
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
                            db.InvIssues.Add(objsave);
                            await db.SaveChangesAsync();
                            break;
                        case (int)ObjectState.Modified:
                            db.Entry(objsave).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            break;
                        case (int)ObjectState.Deleted:
                            var details = db.InvIssueDetails.Where(x => x.InvIssueId == objsave.Id).ToList();
                            foreach (var item in details)
                            {
                                var invissuedetail = await db.InvIssueDetails.FindAsync(item.Id);
                                db.Entry(invissuedetail).State = EntityState.Deleted;
                                db.SaveChanges();
                            }
                            db.Entry(objsave).State = EntityState.Deleted;
                            await db.SaveChangesAsync();
                            break;
                    }
                    vm.Id = objsave.Id;
                    vm.listIssueDetail = await SaveDetailAsync(vm);
                    var result = new { result = vm };
                    return Json(result);
                }
                else
                {
                    var result = new { result = false };
                    return Json(result);
                }

            }
            catch (Exception ex)
            {
                return Json(null);
            }
        }
        public async Task<List<InvIssueDetailDto>> SaveDetailAsync(InvIssueDto vm)
        {
            try
            {
                foreach (var item in vm.listIssueDetail)
                {
                    var objsave = new InvIssueDetail()
                    {
                        Id = item.Id,
                        InventoryItemId = item.InventoryItemId,
                        InvIssueId = vm.Id,
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
                            db.InvIssueDetails.Add(objsave);
                            await db.SaveChangesAsync();
                            break;
                        case (int)ObjectState.Modified:
                            db.Entry(objsave).State = EntityState.Modified;
                            await db.SaveChangesAsync();
                            break;
                    }
                    item.Id = objsave.Id;
                }
                vm.listIssueDetail = GetIssueDetails(vm.Id);
                return vm.listIssueDetail;
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
