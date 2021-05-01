using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.Models
{
    public class RolePermissionViewModel
    {
        public RolePermissionViewModel()
        {
            this.SelectedPermissions = new List<string>();
        }

        public List<UserLevel> UserLevels { get; set; }
        public int LevelId { get; set; }
        public List<AspNetRole> Roles { get; set; }
        public int RoleId { get; set; }
        public List<Menu> Permissions { get; set; }
        public List<string> SelectedPermissions { get; set; }
    }
}