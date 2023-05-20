using System.ComponentModel.DataAnnotations;

namespace WorldDominion.Models
{
    public class Category
    {
        // https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mvc-app/validation?view=aspnetcore-7.0

        [Range(1, 999999, ErrorMessage = "Must be between 1 and 999999")]
        [Display(Name = "Category ID")] // Sets an alias
        public int Id { get; set; } = -1;

        [Required(AllowEmptyStrings = false, ErrorMessage = "You must provide a Category Name")]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = String.Empty;

        // If we want an option to NOT REQUIRED we need to use the ? decorator to indicate the field is nullable
        [Display(Name = "Category Description")]
        public string? Description { get; set; } = String.Empty;
    }
}
