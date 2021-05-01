using RestaurantManagementSystem.Models;
using RestaurantManagementSystem.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagementSystem.Common
{
    public static class CSNCommon
    {

        public static int RestraurantId { get; set; } = 213;
        public static string RestraurantName { get; set; }
        public static bool DataSync { get; set; } = true;
        public static string PNTN { get; set; }
        public static string Address { get; set; }
        public static string Phone1 { get; set; }
        public static string Phone2 { get; set; }
        public static bool DeleteOrderInfoFromTables { get; } = System.Configuration.ConfigurationManager.AppSettings["DeleteOrders"].Equals("1");

        public static ConfigurationViewModel LoadConfigurations()
        {
            ConfigurationViewModel config = new ConfigurationViewModel();
            POSEntities db = new POSEntities();
            var configs = db.Configurations.FirstOrDefault();
            config.DataSync = DataSync = configs.DataSync;
            configs.RestaurantName = RestraurantName = configs.RestaurantName;
            configs.RestrauarntId = RestraurantId = configs.RestrauarntId;
            configs.PNTN = PNTN = configs.PNTN;
            configs.Address = Address = configs.Address;
            configs.Phone1 = Phone1 = configs.Phone1;
            configs.Phone2 = Phone2 = configs.Phone2;
            return config;
        }
        public static IEnumerable<UserPermissionsViewModel> GetUserPermissions(string email)
        {
            POSEntities db = new POSEntities();
            var userPermissions = db.Database.SqlQuery<UserPermissionsViewModel>("Select m.DisplayName,m.Name from MenuAccess ma inner join menu m on ma.MenuId=m.Id " +
                        " INNER JOIN AspNetRoles anr on ma.RoleId = anr.Id " +
                        " INNER join AspNetUserRoles ur on anr.Id = ur.RoleId " +
                        " INNER JOIN AspNetUsers u on u.Id=ur.UserId " +
                        " WHERE u.Email= '" + email + "'");
            return userPermissions;
        }
        public static string getColumnFilterStr<T>(T entityType, HttpRequestBase request)
        {
            var array = typeof(T).GetProperties().Where(p => p.PropertyType.Namespace == "System").Select(property => property.Name).ToArray();

            string qry = "";
            for (int i = 0; i < array.Length; i++)
            {
                if (request["sSearch_" + i + ""] != null && !string.IsNullOrEmpty(request["sSearch_" + i + ""].ToString()))
                    qry += array[i] + " LIKE '%" + request["sSearch_" + i + ""].ToString() + "%' AND ";
            }
            if (!string.IsNullOrEmpty(qry))
                qry = qry.Substring(0, qry.Length - 4);
            return qry;
        }
        public static List<SelectListItem> LocationList
        {
            get
            {
                return new List<SelectListItem>
               {
                    new SelectListItem{Value = "U", Text = "U"},
                    new SelectListItem{ Value = "D", Text = "D" }
                };
            }
        }
    }
}