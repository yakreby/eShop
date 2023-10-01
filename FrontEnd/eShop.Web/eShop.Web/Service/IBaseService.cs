using eShop.Web.Models;

namespace eShop.Web.Service
{
    public interface IBaseService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto);
    }
}
