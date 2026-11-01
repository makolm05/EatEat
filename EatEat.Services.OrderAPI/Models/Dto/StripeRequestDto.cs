namespace EatEat.Services.OrderAPI.Models.Dto
{
    public class StripeRequestDto
    {
        public string StripeSessionID { get; set; } = string.Empty;
        public string StripeSessionUrl { get; set; } = string.Empty;
        public string ApprovedUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public OrderHeaderDto OrderHeader { get; set; }
    }
}
