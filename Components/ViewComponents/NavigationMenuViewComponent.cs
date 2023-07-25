using Microsoft.AspNetCore.Mvc;
using WorldDominion.Models;

namespace WorldDominion.Components.ViewComponents
{
    public class NavigationMenuViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var menuItems = new List<MenuItem>
        {
            new MenuItem { Controller = "Home", Action = "Index", Label = "Home" },
            new MenuItem { Controller = "Shop", Action = "Index", Label = "Shop" },
            new MenuItem { Controller = "Shop", Action = "ViewMyCart", Label = "Cart", Authorized = true },
            new MenuItem { Controller = "Shop", Action = "Orders", Label = "My Orders", Authorized = true, AllowedRoles = new List<string> { "Customer" }},
            new MenuItem { Controller = "Orders", Action = "Orders", Label = "My Orders", Authorized = true, AllowedRoles = new List<string> { "Administrator" }},
            new MenuItem { Controller = "Departments", Action = "Index", Label = "Departments", DropdownItems = new List<MenuItem> {
                new MenuItem { Controller = "Departments", Action = "Index", Label = "List" },
                new MenuItem { Controller = "Departments", Action = "Create", Label = "Create" },
            }, Authorized = true, AllowedRoles = new List<string> { "Administrator" } },
            new MenuItem { Controller = "Products", Action = "Index", Label = "Products", DropdownItems = new List<MenuItem> {
                new MenuItem { Controller = "Products", Action = "Index", Label = "List" },
                new MenuItem { Controller = "Products", Action = "Create", Label = "Create" },
            }, Authorized = true, AllowedRoles = new List<string> { "Administrator" } },
            new MenuItem { Controller = "Home", Action = "About", Label = "About" },
            new MenuItem { Controller = "Home", Action = "Contact", Label = "Contact" },
            new MenuItem { Controller = "Home", Action = "Privacy", Label = "Privacy" },
        };

            return View(menuItems);
        }
    }
}