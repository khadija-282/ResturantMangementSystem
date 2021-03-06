using RestaurantManagementSystem.Toastr;
using System;
using System.Collections.Generic;

namespace RestaurantManagementSystem.Toastr
{
    [Serializable]
    public class Toastr
    {
        public bool ShowNewestOnTop { get; set; }
        public bool ShowCloseButton { get; set; }
        public List<ToastMessage> ToastMessages { get; set; }

        public ToastMessage AddToastMessage(string title, string message, ToastType toastType)
        {
            var toast = new ToastMessage()
            {

                Title = title,
                Message = message,
                ToastType = toastType

            };
            ToastMessages.Add(toast);
            return toast;
        }
        public Toastr()
        {
            ToastMessages = new List<ToastMessage>();
            ShowNewestOnTop = false;
            ShowCloseButton = false;
        }
    }
}