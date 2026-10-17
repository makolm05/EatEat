namespace EatEat.Web.Utility
{
    public static class StaticDetails
    {
        public static string CouponAPIBase { get; set; }
        public static string CouponAPIPath => $"{CouponAPIBase}/api/Coupon/";
        public static string IdentityAPIBase { get; set; }
        public static string IdentityAPIPath => $"{IdentityAPIBase}/api/Identity/";
        public const string TokenCoockie = "JWTToken";

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
    }
}