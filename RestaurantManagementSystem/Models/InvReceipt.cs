//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RestaurantManagementSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class InvReceipt
    {
        public long Id { get; set; }
        public string ReceiptNo { get; set; }
        public Nullable<System.DateTime> ReceiptDate { get; set; }
        public Nullable<long> VendorId { get; set; }
        public Nullable<long> ReceivedBy { get; set; }
        public string Description { get; set; }
        public string ReceivedPlace { get; set; }
    }
}
