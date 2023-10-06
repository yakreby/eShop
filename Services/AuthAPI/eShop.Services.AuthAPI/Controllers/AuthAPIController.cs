﻿
using eShop.Services.AuthAPI.Models.Dto;
using eShop.Services.AuthAPI.RabbitMQSender;
using eShop.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eShop.Services.AuthAPI.Controllers
{
    [Route("api/authAPI")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IRabbitMQAuthMessageSender _rabbitMQAuthMessageSender;
        private readonly IConfiguration _configuration;
        protected ResponseDto _response;

        public AuthAPIController(IAuthService authService, IRabbitMQAuthMessageSender rabbitMQAuthMessageSender, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
            _response = new();
            _rabbitMQAuthMessageSender = rabbitMQAuthMessageSender;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto registrationRequestDto)
        {   
            var errorMessage = await _authService.Register(registrationRequestDto);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            _rabbitMQAuthMessageSender.SendMessage(
                registrationRequestDto.EmailAddress, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var loginResponse = await _authService.Login(loginRequestDto);
            if(loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto roleRegisterationDto)
        {
            var assignRoleSuccessful = await _authService.AssignRole(roleRegisterationDto.EmailAddress, roleRegisterationDto.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }
            return Ok(_response);
        }
    }
}
