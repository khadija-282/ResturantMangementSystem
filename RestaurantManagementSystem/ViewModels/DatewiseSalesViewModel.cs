using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class DatewiseSalesViewModel
    {
        public long Id { get; set; }
        public string BillNumber { get; set; }
        public string Code { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public string Shift { get; set; }
        public string Pax { get; set; }
        public string Amount { get; set; }
        public string GST { get; set; }
        public string SC { get; set; }
        public string Discount { get; set; }
        public string Received { get; set; }
        public string Location { get; set; }
    }
}