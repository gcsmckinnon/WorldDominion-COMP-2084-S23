using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize]
        public async Task<IActionResult> ViewMyCart()
        {
            // Get logged in user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Carts
                .Include(cart => cart.User)
                .Include(cart => cart.CartItems)
                    .ThenInclude(cartItem => cartItem.Product)
                .FirstOrDefaultAsync(cart => cart.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            // Get logged in user
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var cart = await _context.Carts
                .FirstOrDefaultAsync(cart => cart.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == productId);

            if (product == null)
            {
                return NotFound();
            }

            var cartItem = new CartItem
            {
                Cart = cart,
                Product = product,
                Quantity = 1,
                Price = (decimal)product.MSRP
            };

            if (ModelState.IsValid)
            {
                await _context.AddAsync(cartItem);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // If we got this far, something failed. Log the ModelState errors
            var errors = ModelState
                .Where(x => x.Value.Errors.Count > 0)
                .Select(x => new { x.Key, x.Value.Errors })
                .ToArray();

            // You can put a breakpoint here to inspect 'errors' in the debugger
            System.Diagnostics.Debug.WriteLine(errors);

            return View(ProductDetails);
        }
    }
}
