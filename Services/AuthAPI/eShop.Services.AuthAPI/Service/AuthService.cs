using eShop.Services.AuthAPI.Data;
using eShop.Services.AuthAPI.Models;
using eShop.Services.AuthAPI.Models.Dto;
using eShop.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace eShop.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public AuthService(AppDbContext appDbContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<bool> AssignRole(string email, string roleName)
        {
            var user = _appDbContext.ApplicationUsers.FirstOrDefault(x => x.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(roleName).GetAwaiter().GetResult())
                {
                    //Create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(roleName)).GetAwaiter().GetResult();
                }
                await _userManager.AddToRoleAsync(user, roleName);
                return true;
            }
            return false;
        }

        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var user = _appDbContext.ApplicationUsers.FirstOrDefault(x => x.UserName.ToLower() == loginRequestDto.EmailAddress.ToLower());
            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (user == null || isValid == false)
            {
                return new LoginResponseDto() { User = null, Token = "" };
            }
            //if user was found, Generate JWT token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDto = new()
            {
                EmailAddress = user.Email,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber,
                City = user.City
            };

            LoginResponseDto loginResponseDto = new()
            {
                User = userDto,
                Token = token,
            };
            return loginResponseDto;
        }

        public async Task<string> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.EmailAddress,
                Email = registrationRequestDto.EmailAddress,
                NormalizedEmail = registrationRequestDto.EmailAddress.ToUpper(),
                PhoneNumber = registrationRequestDto.PhoneNumber,
                City = registrationRequestDto.City,
                Name = registrationRequestDto.Name,
            };
            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _appDbContext.ApplicationUsers.First(x => x.UserName == registrationRequestDto.EmailAddress);
                    UserDto userDto = new()
                    {
                        Name = userToReturn.Name,
                        EmailAddress = userToReturn.Email,
                        City = userToReturn.City,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    return "";
                }
                else
                {
                    return result.Errors.FirstOrDefault().Description;
                }
            }
            catch (Exception ex)
            {

            }
            return "Error Encountered";
        }
    }
}
