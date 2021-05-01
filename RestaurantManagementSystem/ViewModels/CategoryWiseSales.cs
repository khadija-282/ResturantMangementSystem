using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class CategoryWiseSales
    {
        public string Code { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; } = 0;
        public int NoOfOrders { get; set; } = 0;
    }
}