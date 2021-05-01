using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class ConfigurationViewModel
    {
        public int RestrauarntId { get; set; }
        public string RestaurantName { get; set; }
        public string PNTN { get; set; }
        public string Address { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public bool DataSync { get; set; }
    }
}