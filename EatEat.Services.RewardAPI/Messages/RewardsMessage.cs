namespace EatEat.Services.RewardAPI.Messages
{
    public class RewardsMessage
    {
        public int OrderID { get; set; }
        public string UserID { get; set; } = string.Empty;
        public int RewardsActivity { get; set; }
    }
}
