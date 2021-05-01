using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.ViewModels
{
    public class InvIssueDetailDto
    {
        public long Id { get; set; }
        public long?  InvIssueId { get; set; }
        [Required]
        public long  InventoryItemId { get; set; }
        [Required]
        public double? Quantity { get; set; }
        public string  ItemName { get; set; }
        public string  IssueNo { get; set; }
        public List<ItemDto> listItems { get; set; }
        public int ObjectState { get; set; }
    }
}