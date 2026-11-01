using EatEat.Services.OrderAPI.Models.Dto;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EatEat.Services.OrderAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int ID { get; set; }
        public int OrderHeaderID { get; set; }
        [ForeignKey("OrderHeaderID")]
        public OrderHeader? CartHeader { get; set; }
        public int ProductID { get; set; }
        [NotMapped]
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; } // if product has updated price, we want to show what was a cost when order were
    }
}