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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToCart(int productId, int quantity)
        {
            // Get our logged in user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Attempt to get a cart
            var cart = await _context.Carts
                .FirstOrDefaultAsync(cart => cart.UserId == userId && cart.Active == true);

            // Check that we have don't have an active cart
            if (cart == null)
            {
                cart = new Models.Cart { UserId = userId };
                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            // Find our product
            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == productId);

            // GTFO if no product found
            if (product == null)
            {
                return NotFound();
            }

            // Create a new cart item
            var cartItem = new CartItem
            {
                Cart = cart,
                Product = product,
                Quantity = quantity,
                Price = (decimal)product.MSRP,
            };

            // If valid, do all the goodness
            if (ModelState.IsValid)
            {
                await _context.AddAsync(cartItem);
                await _context.SaveChangesAsync();

                return RedirectToAction("ProductDetails", new { id = productId });
            }

            // Otherwise, GTFO
            return NotFound();
        }
    }
}
