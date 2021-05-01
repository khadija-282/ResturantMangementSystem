using RestaurantManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestaurantManagementSystem.Controllers
{
    public class TreeViewController : Controller
    {
        public POSEntities db = new POSEntities();
        #region Load Tree
        public JsonResult GetRoot()
        {
            List<TreeViewModel> items = GetTree();

            return new JsonResult { Data = items, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
        public JsonResult GetRootWithRole(string roleId)
        {
            List<TreeViewModel> items = GetTreebyRole(roleId);

            return new JsonResult { Data = items, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult GetChildren(string id)
        {
            List<TreeViewModel> items = GetTree(id);

            return new JsonResult { Data = items, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public List<TreeViewModel> GetTreebyRole(string roleId)
        {
           
            var items = new List<TreeViewModel>();

            var roleMenus = db.GetMenusByRoleId(0, roleId);//GET Root Nodes
            //var rootMenus = from m in db.Menus
            //                join a in db.MenuAccesses on m.Id equals a.Id where a.RoleId == roleId && a.HasAccess_YN == true && m.Parent_Menu_Id == null  
            //                 select m; 

            foreach (var m in roleMenus)
            {
                
                TreeViewModel rootNode = new TreeViewModel()
                {
                    text = m.name,
                    icon = m.DisplayName,
                    id = m.id.ToString(),
                    state = new TreeAttribute { id= m.id.ToString(), selected = m.selected == 1 ? true : false }
                   
                    //parent = "#"
                };
                BuildChildNodeByRoleId(rootNode, roleId);
                items.Add(rootNode);
            }
            return items;
        }
        private void BuildChildNodeByRoleId(TreeViewModel rootNode, string roleId)
        {
            if (rootNode != null)
            {
                int p_rootNodeId = Convert.ToInt32(rootNode.id);

                var roleMenus = db.GetMenusByRoleId(p_rootNodeId, roleId);//GET Root Nodes
                List<TreeViewModel> childNodes = new List<TreeViewModel>();
                foreach (var m in roleMenus)
                {
                    TreeViewModel child = new TreeViewModel();
                    child.text = m.name;
                    child.icon = m.DisplayName;
                    child.id = m.id.ToString();
                    child.state = new TreeAttribute { id = m.id.ToString(), selected = m.selected == 1 ? true : false };
                    childNodes.Add(child);

                }
               
                if (childNodes.Count > 0)
                {
                    foreach (var childRootNode in childNodes)
                    {
                        BuildChildNodeByRoleId(childRootNode, roleId);
                        rootNode.children.Add(childRootNode);
                    }
                }
            }
        }

        public List<TreeViewModel> GetTree()
        {
            var items = new List<TreeViewModel>();

            var rootMenus = db.Menus.Where(a => a.Parent_Menu_Id == null);
            foreach (Menu m in rootMenus)
            {

                TreeViewModel rootNode = new TreeViewModel()
                {
                    text = m.Name,
                    icon = m.DisplayName,
                    id = m.Id.ToString()
                    //parent = "#"
                };
                BuildChildNode(rootNode);
                items.Add(rootNode);
            }
            return items;
        }

        public List<TreeViewModel> GetTree(string id)
        {
            long menuId = Convert.ToInt64(id);
            var items = new List<TreeViewModel>();

            var rootMenus = db.Menus.Where(a => a.Parent_Menu_Id == null).Where (a => a.Id == menuId);

            foreach (Menu m in rootMenus)
            {
                TreeViewModel rootNode = new TreeViewModel()
                {
                    text = m.Name,
                    icon = m.DisplayName,
                    id = m.Id.ToString()
                    // parent = "#"
                };
                BuildChildNode(rootNode);
                items.Add(rootNode);
            }


            return items;
        }


        private void BuildChildNode(TreeViewModel rootNode)
        {
            if (rootNode != null)
            {
                long rootNodeId = Convert.ToInt64(rootNode.id);
                List<TreeViewModel> childNode = (from m in db.Menus
                                                 where m.Parent_Menu_Id == rootNodeId
                                                 select new TreeViewModel()
                                                 {
                                                     text = m.Name,
                                                     icon = m.DisplayName,
                                                     id = m.Id.ToString(),
                                                     // parent = rootNode.text
                                                 }).ToList<TreeViewModel>();
                if (childNode.Count > 0)
                {
                    foreach (var childRootNode in childNode)
                    {
                        BuildChildNode(childRootNode);
                        rootNode.children.Add(childRootNode);
                    }
                }
            }
        }
        #endregion

    }
}