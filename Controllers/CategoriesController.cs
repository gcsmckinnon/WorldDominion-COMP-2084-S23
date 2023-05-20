using Microsoft.AspNetCore.Mvc;
using WorldDominion.Models;

namespace WorldDominion.Controllers
{
    public class CategoriesController : Controller
    {
        public static List<Category> Categories { get; set; }

        public CategoriesController()
        {
            if (Categories == null)
            {
                Categories = new List<Category>()
                {
                    new Category { Id = 1, Name = "Dairy and Eggs", Description = "Find all the best milks and ovums" },
                    new Category { Id = 2, Name = "Fruits and Vegetables", Description = "Lettuce turn up the beet! It's kind of a big dill." },
                    new Category { Id = 3, Name = "Meats", Description = "We're a purveyor of fine meats." },
                };
            }
        }

        public IActionResult Index()
        {
            return View(Categories);
        }

        public IActionResult Browse(int Id)
        {
            var category = Categories.Find(category => category.Id == Id);
            return View(category);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                Categories.Add(category);
                
                return RedirectToAction("Index");
            }

            return View(category);
        }
    }
}
