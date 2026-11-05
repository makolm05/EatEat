namespace EatEat.Services.OrderAPI.Models.Dto
{
    public class RewardsDto
    {
        public int OrderID { get; set; }
        public string UserID { get; set; } = string.Empty;
        public int RewardsActivity { get; set; }
    }
}
