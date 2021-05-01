using RestaurantManagementSystem.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class InvReceiptDto
    {
        public InvReceiptDto()
        {
            listDetail = new List<InvReceiptDetailDto>();
            listUser = new List<UserDto>();
            listDetailToDelete = new List<int>();
            listVendor = new List<VendorDto>();
        }
        public long Id { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string Description { get; set; }
        public long? VendorId { get; set; }
        public  long? ReceivedById { get; set; }
        public string VendorName { get; set; }
        public string ReceivedBy { get; set; }
        public List<InvReceiptDetailDto> listDetail { get; set; }
        public List<int> listDetailToDelete { get; set; }
        public List<UserDto> listUser { get; set; }
        public List<VendorDto> listVendor { get; set; }
        public List<RecievedPlaceDto> listReceivedPlace{ get; set; }
        public string ReceivedPlace { get; set; }
        public int ReceivedPlaceId { get; set; }

        public int ObjectState { get; set; }

    }
    public class InvReceiptDetailDto
    {
        public InvReceiptDetailDto()
        {
            listItems = new List<ItemDto>();
        }
        public long DetailId { get; set; }
        public long? InvReceiptId { get; set; }
        public long? InventoryItemId { get; set; }
        public double? Quantity { get; set; }
        public double? Price { get; set; }
        public string ItemName { get; set; }
        public int ObjectState { get; set; }
        public List<ItemDto> listItems { get; set; }

    }
    public class RecievedPlaceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public static List<RecievedPlaceDto> ConvertEnum()
        {
            var list = new List<RecievedPlaceDto>();
            foreach (RecievePlace foo in Enum.GetValues(typeof(RecievePlace)))
            {
                var obj = new RecievedPlaceDto()
                {
                    Id = (int)foo,
                    Name = foo.ToString()
                };
                list.Add(obj);
            }
            return list;
        }
    }
}