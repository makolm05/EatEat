namespace EatEat.Web.Utility
{
    public static class StaticDetails
    {
        public static string ProductAPIBase { get; set; } = string.Empty;
        public static string ProductAPIPath => $"{ProductAPIBase}/api/Product/";
        public static string CouponAPIBase { get; set; } = string.Empty;
        public static string CouponAPIPath => $"{CouponAPIBase}/api/Coupon/";
        public static string IdentityAPIBase { get; set; } = string.Empty;
        public static string IdentityAPIPath => $"{IdentityAPIBase}/api/Identity/";
        public static string ShoppingCartAPIBase { get; set; } = string.Empty;
        public static string ShoppingCartAPIPath => $"{ShoppingCartAPIBase}/api/Cart/";
        public static string OrderAPIBase { get; set; } = string.Empty;
        public static string OrderAPIPath => $"{OrderAPIBase}/api/Order/";
        public const string TokenCookie = "JWTToken";

        public enum APIType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
        public enum APIRoles
        {
            Admin,
            Customer
        }

        public const string AdminRole = "ADMIN";
        public const string CustomerRole = "CUSTOMER";
    }
}