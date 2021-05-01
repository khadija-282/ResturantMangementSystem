using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.Models
{
    public class RecipeViewModel
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public long? MenuItemId { get; set; }
        public long? InventoryItemId { get; set; }
        public double? Quantity { get; set; }
    }
}