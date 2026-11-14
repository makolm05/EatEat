using EatEat.Web.Utility;
using System.ComponentModel.DataAnnotations;

namespace EatEat.Web.Models.Dto
{
    public class ProductDto
    {
        public int ID { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string CategoryName { get; set; } = string.Empty;
        [Required]
        public double Price { get; set; }
        [Required]
        public string Description { get; set; } = string.Empty;
        public string? ImageUrl { get; set; } = string.Empty;
        public string? ImageLocalPath { get; set; } = string.Empty;
        [Range(1, 100)]
        public int Count { get; set; } = 1;
        [ImageMaxSize(2 * 1024 * 1024)] // 2 MB
        [AllowedImageExtensions(new string[] { ".jpg", ".jpeg", ".png", ".gif" })]
        public IFormFile? Image { get; set; }
    }
}