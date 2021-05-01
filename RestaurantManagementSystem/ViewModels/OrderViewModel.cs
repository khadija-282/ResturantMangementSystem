using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class OrderViewModel
    {
        public OrderViewModel()
        {
            OrderDetail = new List<OrderDetailViewModel>();
        }
        public long? Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public DateTime? OrderDateTime { get; set; }
        public DateTime? ExpectedServingTime { get; set; }
        public long? CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public long? StatusId { get; set; }
        public string TableNumber { get; set; }
        public string Shift { get; set; }
        public string Location { get; set; }
        public long? WaiterId { get; set; }
        public string Waiter { get; set; }
        public string OrderStatus { get; set; }
        public decimal? GrandTotal { get; set; }
        public decimal? DiscountValue { get; set; }
        public decimal? GST { get; set; }
        public decimal? ServiceCharges { get; set; }
        public decimal? NetTotal { get; set; }
        public int? Pax { get; set; }
        public string BillNumber { get; set; }
        public bool IsNewOrder { get; set; } = true;
        public List<OrderDetailViewModel> OrderDetail { get; set; }
    }
}