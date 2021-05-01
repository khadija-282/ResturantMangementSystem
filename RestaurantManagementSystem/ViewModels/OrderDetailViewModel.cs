using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class OrderDetailViewModel
    {
        public long? Id { get; set; }
        public long? OrderId { get; set; }
        public long ItemId { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public decimal? VAT { get; set; }
        public string DiscountType { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? ServiceCharges { get; set; }
        public decimal GrandTotal { get; set; }
        public int? Pax { get; set; }
    }
}