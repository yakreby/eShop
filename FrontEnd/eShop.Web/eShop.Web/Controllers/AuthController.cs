using eShop.Web.Models;
using eShop.Web.Service.IService;
using eShop.Web.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace eShop.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto loginRequestDto = new();
            return View(loginRequestDto);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto loginRequestDto)
        {
            ResponseDto responseDto = await _authService.LoginAsync(loginRequestDto);
            if (responseDto != null && responseDto.IsSuccess)
            {
                LoginResponseDto loginResponseDto =
                    JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(responseDto.Result));
                if(User.Identity.IsAuthenticated){

                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", responseDto.Message);
                return View(loginRequestDto);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = new List<SelectListItem>
            {
                new SelectListItem { Text = StaticDetails.Role_Admin, Value = StaticDetails.Role_Admin},
                new SelectListItem { Text = StaticDetails.Role_Customer, Value = StaticDetails.Role_Customer}
            };

            ViewBag.RoleList = roleList;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegistrationRequestDto registrationRequestDto)
        {
            ResponseDto result = await _authService.RegisterAsync(registrationRequestDto);
            ResponseDto assignRole;
            if (result != null && result.IsSuccess)
            {
                if (string.IsNullOrEmpty(registrationRequestDto.Role))
                {
                    registrationRequestDto.Role = StaticDetails.Role_Customer;
                }
                else
                {
                    assignRole = await _authService.AssignRoleAsync(registrationRequestDto);
                    if (assignRole != null && assignRole.IsSuccess)
                    {
                        TempData["Success"] = "Registration Success";
                        return RedirectToAction(nameof(Login));
                    }
                }
            }

            var roleList = new List<SelectListItem>
            {
                new SelectListItem { Text = StaticDetails.Role_Admin, Value = StaticDetails.Role_Admin},
                new SelectListItem { Text = StaticDetails.Role_Customer, Value = StaticDetails.Role_Customer}
            };
            ViewBag.RoleList = roleList;
            return View(registrationRequestDto);
        }

        [HttpGet]
        public IActionResult Logout()
        {
            return View();
        }
    }
}
