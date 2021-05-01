using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RestaurantManagementSystem.Models
{
    public class RoleViewModel
    {
        public int Id { get; set; }
        public long? LevelId { get; set; }
        public string LevelName { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string DisplayName { get; set; }

        public string Name { get; set; }

        public void setRoleName(string DisplayName, string LevelName)
        {
            this.Name = DisplayName + "-" + LevelName;
        }
        public void setDisplayName(string RoleName)
        {
            int len = RoleName.LastIndexOf("-") > 0 ? RoleName.LastIndexOf("-") : RoleName.Length;
            this.DisplayName = RoleName.Substring(0, len);
        }



    }
}