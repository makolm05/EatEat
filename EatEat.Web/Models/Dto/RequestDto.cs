using static EatEat.Web.Utility.StaticDetails;

namespace EatEat.Web.Models.Dto
{
    public class RequestDto
    {
        public APIType APIType { get; set; } = APIType.GET;
        public string Url { get; set; } = string.Empty;
        public object? Data { get; set; }
        public string AccessToken { get; set; } = string.Empty;
    }
}