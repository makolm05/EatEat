namespace EatEat.Services.ShoppingCartAPI.Models.Dto
{
    public class ApplyCouponDto
    {
        public string UserID { get; set; } = string.Empty;
        public string CouponCode { get; set; } = string.Empty;
    }
}