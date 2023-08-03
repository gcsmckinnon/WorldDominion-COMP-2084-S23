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
            new MenuItem { Controller = "Shop", Action = "Orders", Label = "My Orders", Authorized = true },
            new MenuItem { Controller = "Orders", Action = "Orders", Label = "Admin", Authorized = true, AllowedRoles = new List<string> { "Administrator" }, DropdownItems = new List<MenuItem> {
                new MenuItem { Controller = "Orders", Action = "Index", Label = "Orders" },
                new MenuItem { Controller = "Carts", Action = "Index", Label = "Carts" },
                new MenuItem { Controller = "Departments", Action = "Index", Label = "Departments" },
                new MenuItem { Controller = "Products", Action = "Index", Label = "Products" },
            }},
            new MenuItem { Controller = "Home", Action = "About", Label = "About" },
            new MenuItem { Controller = "Home", Action = "Contact", Label = "Contact" },
            new MenuItem { Controller = "Home", Action = "Privacy", Label = "Privacy" },
        };

            return View(menuItems);
        }
    }
}