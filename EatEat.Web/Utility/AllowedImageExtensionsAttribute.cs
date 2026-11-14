using System.ComponentModel.DataAnnotations;

namespace EatEat.Web.Utility
{
    public class AllowedImageExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions;
        public AllowedImageExtensionsAttribute(string[] allowedExtensions)
        {
            _allowedExtensions = allowedExtensions;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult("Invalid file extension.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
