using eShop.Web.Models.Dto;
using eShop.Web.Service.Base;

namespace eShop.Web.Service.IService
{
    public interface IBaseService : IScopedService
    {
        Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true);
    }
}
