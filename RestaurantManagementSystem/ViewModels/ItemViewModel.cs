using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class ItemViewModel
    {
        public long Id { get; set; }
        public long Ser_No { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description{ get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public bool? IsDealItem_YN { get; set; }
    }
}