using eShop.Web.Models.Dto;
using eShop.Web.Service.Base;

namespace eShop.Web.Service.IService
{
    public interface ICartService : IScopedService
    {
        Task<ResponseDto>? GetCartByUserIdAsync(string userId);
        Task<ResponseDto>? UpsertCartAsync(CartDto cart);
        Task<ResponseDto>? RemoveFromCartAsync(int cartDetailsId);
        Task<ResponseDto>? ApplyCouponAsync(CartDto cartDto);
        Task<ResponseDto?> EmailCart(CartDto cart);
    }
}
