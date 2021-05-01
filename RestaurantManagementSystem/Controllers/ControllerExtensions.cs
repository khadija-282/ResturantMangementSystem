using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestaurantManagementSystem.Toastr;

using System.Web.Mvc;

namespace RestaurantManagementSystem.Controllers
{
    public static class ControllerExtensions
    {
        public static ToastMessage AddToastMessage(this Controller controller, string title, string message, ToastType toastType = ToastType.Info)
        {
            Toastr.Toastr toastr = controller.TempData["Toastr"] as Toastr.Toastr;
            toastr = toastr ?? new Toastr.Toastr();

            var toastMessage = toastr.AddToastMessage(title, message, toastType);
            controller.TempData["Toastr"] = toastr;
            return toastMessage;
        }
    }
}