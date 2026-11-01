namespace EatEat.Services.OrderAPI.Models.Dto
{
    public class OrderDetailsDto
    {
        public int ID { get; set; }
        public int OrderHeaderID { get; set; }
        public int ProductID { get; set; }
        public ProductDto? Product { get; set; }
        public int Count { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public double ProductPrice { get; set; } // if product has updated price, we want to show what was a cost when order were
    }
}