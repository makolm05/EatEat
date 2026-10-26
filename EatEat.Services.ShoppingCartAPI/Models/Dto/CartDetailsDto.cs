namespace EatEat.Services.ShoppingCartAPI.Models.Dto
{
    public class CartDetailsDto
    {
        public int ID { get; set; }
        public int CartHeaderID { get; set; }
        public CartHeaderDto? CartHeader { get; set; }
        public int ProductID { get; set; }
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
    }
}