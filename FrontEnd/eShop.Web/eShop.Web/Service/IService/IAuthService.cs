using eShop.Web.Models;
using eShop.Web.Service.Base;

namespace eShop.Web.Service.IService
{
    public interface IAuthService : IScopedService
    {   
        Task<ResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<ResponseDto> AssignRoleAsync(RegistrationRequestDto registrationRequestDto);
    }
}
