using System.ComponentModel.DataAnnotations;

namespace WorldDominion.Models
{
    public class Category
    {
        // https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/validation?view=aspnetcore-7.0

        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must provide a Category Name"), MaxLength(100)]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        // If we want an option to NOT REQUIRED we need to use the ? decorator to indicate the field is nullable
        [Display(Name = "Category Description")]
        public string? Description { get; set; }


        // Associative References
        public List<Product>? Products { get; set; } // Child reference
    }
}
