using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorldDominion.Data;
using WorldDominion.Models;

namespace WorldDominion.Controllers
{
    public class ShopController : Controller
    {
        // Property for our database connection
        private ApplicationDbContext _context;

        // Constructor
        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var departments = await _context.Departments
                .OrderBy(department => department.Name)
                .ToListAsync();

            return View(departments);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var departmentWithProducts = await _context.Departments
                .Include(department => department.Products)
                .FirstOrDefaultAsync(department => department.Id == id);

            return View(departmentWithProducts);
        }

        public async Task<IActionResult> ProductDetails(int? id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == id);
            
            return View(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            // Get our logged in user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Attempt to get a cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            // If there's no cart, then we must create one
            if (cart == null)
            {
                cart = new Cart { UserId = userId };

                if (!ModelState.IsValid) return NotFound();

                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            // Fetch the requested product
            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == productId);

            if (product == null) return NotFound();

            // Create our cart item
            var cartItem = new CartItem
            {
                Cart = cart,
                Product = product,
                Quantity = quantity,
                Price = product.MSRP
            };

            if (!ModelState.IsValid) return NotFound();

            await _context.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewMyCart");
        }

        [Authorize]
        public async Task<IActionResult> ViewMyCart()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Carts
                .Include(cart => cart.User)
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            return View(cart);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> DeleteCartItem(int cartItemId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Carts
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            if (cart == null) return NotFound();

            var cartItem = await _context.CartItems
                .Include(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cartItem => cartItem.Cart == cart && cartItem.Id == cartItemId);

            if (cartItem == null) return NotFound();

            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("ViewMyCart");
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Carts
                .Include(cart => cart.User)
                .Include(cart => cart.CartItems)
                .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            if (cart == null) return NotFound();

            var order = new Order
            {
                UserId = userId,
                Cart = cart,
                Total = cart.CartItems.Sum(cartItem => cartItem.Price * cartItem.Quantity),
                ShippingAddress = null,
                PaymentMethod = PaymentMethods.VISA,
            };

            ViewData["PaymentMethods"] = new SelectList(Enum.GetValues(typeof(PaymentMethods)));

            return View(order);
        }
    }
}
