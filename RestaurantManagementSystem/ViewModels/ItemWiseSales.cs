using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class ItemWiseSales
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string CategoryCode { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; } = 0;
        public int NoOfOrders { get; set; } = 0;
        public string Shift { get; set; }
        public string Location { get; set; }
    }
}