using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.Models
{
    public class SearchContactsViewModel
    {
        public Int64 SrNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string JobTitle { get; set; }
        public string WorkPhone { get; set; }
        public string HomePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public Int64 CountryId { get; set; }
        public Int64 StateId { get; set; }
        public Int64 CityId { get; set; }
        public string Address { get; set; }
    }
}