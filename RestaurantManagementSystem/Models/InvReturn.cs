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
    
    public partial class InvReturn
    {
        public long Id { get; set; }
        public string ReturnNo { get; set; }
        public Nullable<System.DateTime> ReturnDate { get; set; }
        public Nullable<long> ReturnBy { get; set; }
        public Nullable<long> ReturnTo { get; set; }
        public string Description { get; set; }
    }
}
