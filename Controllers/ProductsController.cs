using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldDominion.Data;
using WorldDominion.Models;

namespace WorldDominion.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Products.Include(p => p.Department).OrderBy(p => p.Name);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            // If there are no categories, we don't want to let the user create a product
            if (!_context.Departments.Any())
            {
                ModelState.AddModelError("", "No categories exist. Please create a category first.");
                return RedirectToAction("Index", "Departments");
            }
            
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["WeightUnit"] = new SelectList(Enum.GetValues(typeof(ProductWeightUnit)));

            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,DepartmentId,Name,Description,MSRP,Weight,WeightUnit")] Product product, IFormFile? Photo)
        {
            if (ModelState.IsValid)
            {
                // Check for photo upload and save the file
                product.Photo = await UploadPhoto(Photo);

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", product.DepartmentId);
            ViewData["WeightUnit"] = new SelectList(Enum.GetValues(typeof(ProductWeightUnit)));

            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", product.DepartmentId);
            ViewData["WeightUnit"] = new SelectList(Enum.GetValues(typeof(ProductWeightUnit)));

            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,DepartmentId,Name,Description,MSRP,Weight,WeightUnit")] Product product, IFormFile? Photo)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    product.Photo = await UploadPhoto(Photo);

                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", product.DepartmentId);
            ViewData["WeightUnit"] = new SelectList(Enum.GetValues(typeof(ProductWeightUnit)));

            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Department)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Products == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Products'  is null.");
            }
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
          return (_context.Products?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private async Task<string> UploadPhoto(IFormFile Photo)
        {
            if (Photo != null)
            {
                // Get temp location
                var filePath = Path.GetTempFileName();

                // Create a unique name so we don't overwrite any existing photos
                // eg: photo.jpg => abcdefg123456890-photo.jpg (Using the Globally Unique Identifier (GUID))
                var fileName = Guid.NewGuid() + "-" + Photo.FileName;

                // Set destination path dynamically so it works on any system (double slashes escape the characters)
                var uploadPath = System.IO.Directory.GetCurrentDirectory() + "\\wwwroot\\img\\products\\" + fileName;

                // Execute the file copy
                using var stream = new FileStream(uploadPath, FileMode.Create);
                await Photo.CopyToAsync(stream);

                // Set the Photo property name of the new Product object
                return fileName;
            }

            return null;
        }
    }
}
