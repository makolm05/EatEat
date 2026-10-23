using System.Text;
using Newtonsoft.Json;
using EatEat.Web.Models.Dto;
using EatEat.Web.Services.Interfaces;
using static EatEat.Web.Utility.StaticDetails;
using System.Net;

namespace EatEat.Web.Services
{
    public class BaseService : IBaseService
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(ITokenProvider tokenProvider, IHttpClientFactory httpClientFactory)
        {
            _tokenProvider = tokenProvider;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto request, bool withBearer)
        {
            try
            {
                HttpClient client = _httpClientFactory.CreateClient("EatEatAPI");

                using var message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                
                //token
                if (withBearer)
                    message.Headers.Add("Authorization", $"Bearer {_tokenProvider.GetToken()}");

                message.RequestUri = new Uri(request.Url);
                if (request.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request.Data), Encoding.UTF8, "application/json");
                }
                message.Method = GetHttpMethod(request.APIType);

                using var response = await client.SendAsync(message);
                return await GetResponse(response);
            }
            catch(Exception ex)
            {
                var dto = new ResponseDto
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
                return dto;
            }
        }

        protected HttpMethod GetHttpMethod(APIType apiType)
        {
            return apiType switch
            {
                APIType.POST => HttpMethod.Post,
                APIType.PUT => HttpMethod.Put,
                APIType.DELETE => HttpMethod.Delete,
                _ => HttpMethod.Get,
            };
        }

        protected async Task<ResponseDto> GetResponse(HttpResponseMessage response)
        {
            switch (response.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Resource not found."
                    };
                case HttpStatusCode.Unauthorized:
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Unauthorized."
                    };
                case HttpStatusCode.Forbidden:
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Access Denied."
                    };
                case HttpStatusCode.InternalServerError:
                    return new ResponseDto
                    {
                        IsSuccess = false,
                        Message = "Internal Server Error."
                    };
                default:
                    var apiContent = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            }
        }

    }
}
