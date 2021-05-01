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
    
    public partial class InventoryItem
    {
        public long Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public string PackagingUnit { get; set; }
        public string RecipeUnit { get; set; }
        public Nullable<double> ReciptUnitPerPackage { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public Nullable<int> ReOrderLevel { get; set; }
        public Nullable<double> ReplenishLevel { get; set; }
        public Nullable<double> TotalPackages { get; set; }
        public Nullable<bool> UpdateRecipeBalance { get; set; }
        public Nullable<double> PackagePuchasePrice { get; set; }
        public Nullable<double> PackageSellingPrice { get; set; }
        public Nullable<long> Group { get; set; }
        public Nullable<long> LocationId { get; set; }
        public Nullable<long> VendorId { get; set; }
    }
}