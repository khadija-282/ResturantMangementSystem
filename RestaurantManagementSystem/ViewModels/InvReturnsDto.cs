using RestaurantManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class InvReturnsDto
    {
        public InvReturnsDto()
        {
            listReturnDetails = new List<InvReturnsDetailDto>();
            listReturnDetailsToDelete = new List<int>();
            listUser = new List<UserDto>();
        }
        public long Id { get; set; }
        public string ReturnNo { get; set; }
        public DateTime? ReturnDateTime { get; set; }
        public long? ReturnBy { get; set; }
        public long? ReturnTo { get; set; }
        public string Description { get; set; }
        public string ReturnByEmail { get; set; }
        public string ReturnToEmail { get; set; }
        public int objectState { get; set; }
        public List<UserDto> listUser { get; set; }
        public List<int> listReturnDetailsToDelete { get; set; }
        public List<InvReturnsDetailDto> listReturnDetails { get; set; }

    }
}