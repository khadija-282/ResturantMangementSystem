using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.Models
{
    public class VoterViewModel
    {
        public long Id { get; set; }
        public long Ser_No { get; set; }
        public string Name { get; set; }
        public string FatherName { get; set; }
        public string MobileNo { get; set; }
        public string CNIC { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
    }
}