using System.ComponentModel.DataAnnotations;

namespace EatEat.Web.Utility
{
    public class ImageMaxSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public ImageMaxSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult($"File size exceeds the maximum allowed {_maxFileSize} bytes size.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
