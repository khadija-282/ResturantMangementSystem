using System.Collections.Generic;

namespace RestaurantManagementSystem.ViewModels
{
    public class InvReturnsDetailDto
    {
        public long Id { get; set; }
        public long? InvReturnId { get; set; }
        public long? InventoryItemId { get; set; }
        public double? Quantity { get; set; }
        public string ReturnNo { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }
        public int objectState { get; set; }
        public List<ItemDto> listItems { get; set; }
    }
}