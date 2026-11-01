using System.ComponentModel.DataAnnotations;

namespace EatEat.Web.Models.Dto
{
    public class CartHeaderDto
    {
        public int ID { get; set; }
        public string? UserID { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string Phone { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        public string? CouponCode { get; set; }
        public double Discount { get; set; }
        public double CartTotal { get; set; }
    }
}