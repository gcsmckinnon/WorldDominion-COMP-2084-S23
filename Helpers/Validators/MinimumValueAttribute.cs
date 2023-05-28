using System.ComponentModel.DataAnnotations;

namespace WorldDominion.Helpers.Validators
{
    public class MinimumValueAttribute : ValidationAttribute
    {
        private readonly double _minValue;

        public MinimumValueAttribute(double minValue)
        {
            _minValue = minValue;
        }

        public override bool IsValid(object value)
        {
            return value is double number && number >= _minValue;
        }
    }
}
