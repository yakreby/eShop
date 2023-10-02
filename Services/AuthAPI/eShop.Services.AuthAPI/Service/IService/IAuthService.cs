using eShop.Services.AuthAPI.Models.Dto;

namespace eShop.Services.AuthAPI.Service.IService
{
    public interface IAuthService
    {
        Task<UserDto> Register(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> CreateAsync(LoginRequestDto loginRequestDto);
    }
}
