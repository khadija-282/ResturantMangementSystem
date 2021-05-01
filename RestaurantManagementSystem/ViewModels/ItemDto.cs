using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class ItemDto
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}