using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestaurantManagementSystem.Models
{
    public class TreeViewModel
    {
        public TreeViewModel()
        {
            children = new List<TreeViewModel>();
        }

        public string id { get; set; }
        public string text { get; set; }
        public string icon { get; set; }
        public TreeAttribute state { get; set; }
      
       // public string parent { get; set; }
    
        public IList<TreeViewModel> children { get; set; }

     
    }
    public class TreeAttribute
    {
        public string id;
        public bool selected;
    }
}