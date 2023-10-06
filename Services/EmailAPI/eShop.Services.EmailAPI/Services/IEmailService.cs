using eShop.Services.EmailAPI.Models;
using eShop.Services.EmailAPI.Models.Dto;

namespace eShop.Services.EmailAPI.Services
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);
        Task RegisterUserEmailAndLog(string email);
        Task LogOrderPlaced(RewardsMessage rewardsDto);
    }
}
