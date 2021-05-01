using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class DashboardViewModel
    {
        public MonthlySales MonthlySales { get; set; }
        public TodaysSales TodaysSales { get; set; }
        public MonthHotItem MonthHotItem { get; set; }
        public TodaysHotItem TodaysHotItem { get; set; }
        public List<TodaysSalesDetail> TodaysSalesDetail{ get; set; }
    }
    public class MonthlySales
    {
        public decimal? Sales { get; set; } = 0;
        public decimal? GST { get; set; } = 0;
        public decimal? ServiceCharges { get; set; } = 0;
    }
    public class TodaysSales
    {
        public decimal? Sales { get; set; } = 0;
        public decimal? GST { get; set; } = 0;
        public decimal? ServiceCharges { get; set; } = 0;
    }
    public class MonthHotItem
    {
        public long? Id { get; set; } = 0;
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Quantity { get; set; } = 0;

    }
    public class TodaysHotItem
    {
        public long? Id { get; set; } = 0;
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal? Quantity { get; set; } = 0;

    }
    public class TodaysSalesDetail
    {
        public string OrderNumber { get; set; }
        public long? InvoiceNumber { get; set; } = 0;
        public DateTime? OrderDateTime { get; set; } = DateTime.Now;
        public string TableNumber { get; set; }
        public string OrderStatus { get; set; }
        public decimal? Total { get; set; } = 0;
        public decimal? GST { get; set; } = 0;
        public decimal? NetTotal { get; set; } = 0;
    }
}