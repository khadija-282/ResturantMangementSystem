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
    
    public partial class UserLevel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserLevel()
        {
            this.AspNetRoles = new HashSet<AspNetRole>();
        }
    
        public long LevelId { get; set; }
        public string LevelName { get; set; }
        public string Description { get; set; }
        public Nullable<bool> IsActive_YN { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
    }
}
