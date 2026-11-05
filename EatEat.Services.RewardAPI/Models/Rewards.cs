namespace EatEat.Services.RewardAPI.Models
{
    public class Rewards
    {
        public int ID { get; set; }
        public string UserID { get; set; } = string.Empty;
        public int OrderID { get; set; }
        public int RewardsActivity { get; set; }
        public DateTime RewardsDate { get; set; }
    }
}
