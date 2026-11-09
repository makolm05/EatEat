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
        public string? ImagLocalPath { get; set; } = string.Empty;
        [Range(1, 100)]
        public int Count { get; set; } = 1;
        public IFormFile? Image { get; set; }
    }
}