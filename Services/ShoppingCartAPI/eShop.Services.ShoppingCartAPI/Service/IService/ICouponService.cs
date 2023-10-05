using eShop.Services.ShoppingCartAPI.Models.Dto;

namespace eShop.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponAsync(string couponCode);
    }
}
