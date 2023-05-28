using System.ComponentModel.DataAnnotations;
using WorldDominion.Helpers.Validators;

namespace WorldDominion.Models
{
    public enum ProductWeightUnit
    {
        Grams,
        Kilograms,
        Pounds,
        Ounces,
        Litres,
    }

    public class Product
    {
        public int Id { get; set; }

        [Display(Name="Category")]
        public int CategoryId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        [MinimumValue(0.01), Required]
        public double MSRP { get; set; }

        public decimal Weight { get; set; }

        public ProductWeightUnit WeightUnit { get; set; }

        public string? Photo { get; set; }

        
        // Associative References (Navigation Properties)
        public Category? Category { get; set; } // Parent reference
    }
}
