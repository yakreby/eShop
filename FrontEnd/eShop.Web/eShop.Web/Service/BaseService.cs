using eShop.Web.Models;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using static eShop.Web.Utility.StaticDetails;

namespace eShop.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public BaseService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<ResponseDto?> SendAsync(RequestDto requestDto)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("eShop");
            HttpRequestMessage requestMessage = new();
            requestMessage.Headers.Add("Accept", "application/json");
            //Token

            requestMessage.RequestUri = new Uri(requestDto.Url);
            if (requestDto.Url != null)
            {
                requestMessage.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data),
                    Encoding.UTF8, "application/json");
            }
            HttpResponseMessage apiResponse = null;

            switch (requestDto.ApiType)
            {
                //case ApiType.GET:
                //    requestMessage.Method = HttpMethod.Get;
                //    break;
                case ApiType.POST:
                    requestMessage.Method = HttpMethod.Post;
                    break;
                case ApiType.PUT:
                    requestMessage.Method = HttpMethod.Put;
                    break;
                case ApiType.DELETE:
                    requestMessage.Method = HttpMethod.Delete;
                    break;
                default:
                    requestMessage.Method = HttpMethod.Get;
                    break;
            }

            apiResponse = await httpClient.SendAsync(requestMessage);
            try
            {
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Interal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
