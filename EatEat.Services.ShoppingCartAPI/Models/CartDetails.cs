using System.ComponentModel.DataAnnotations;
using EatEat.Services.ShoppingCartAPI.Models.Dto;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatEat.Services.ShoppingCartAPI.Models
{
    public class CartDetails
    {
        [Key]
        public int ID { get; set; }
        public int CartHeaderID { get; set; }
        [ForeignKey(nameof(CartHeaderID))]
        public CartHeader CartHeader { get; set; }
        public int ProductID { get; set; }
        [NotMapped]
        public ProductDto Product { get; set; }
        public int Count { get; set; }
    }
}