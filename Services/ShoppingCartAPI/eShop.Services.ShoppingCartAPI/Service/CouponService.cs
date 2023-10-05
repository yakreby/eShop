using eShop.Services.ShoppingCartAPI.Models.Dto;
using eShop.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace eShop.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CouponDto> GetCouponAsync(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var responseBack = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (responseBack.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(responseBack.Result));
            }
            return new CouponDto();
        }
    }
}
