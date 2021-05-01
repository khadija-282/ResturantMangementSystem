using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.Enums
{
    public enum EOrderStatus
    {
        NEW = 1,
        PENDING = 2,
        COMPLETED = 5,
        DELELETED = 6,
        CANCELLED = 7
    }
    public enum ObjectState
    {
        Unchanged = 0,
        Added = 1,
        Modified = 2,
        Deleted = 3
    }
    public enum IssuePlace
    {
        Kitchen=1,
        OtherOption=2
    }
    public enum RecievePlace
    {
        Kitchen = 1,
        Vendor = 2
    }
}