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
            if (User == null) return NotFound();

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var cart = await _context.Carts
                .FirstOrDefaultAsync(cart => cart.UserId == userId);

            if (cart == null)
            {
                cart = new Cart { UserId = userId };

                if (!ModelState.IsValid) return NotFound();

                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(product => product.Id == productId);

            if (product == null) return NotFound();

            var cartItem = new CartItem
            {
                Cart = cart,
                Product = product,
                Quantity = quantity,
                Price = (decimal)product.MSRP
            };

            if (!ModelState.IsValid) return NotFound();

            await _context.AddAsync(cartItem);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
