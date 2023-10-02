namespace eShop.Services.AuthAPI.Models.Dto
{
    public class RegistrationRequestDto
    {
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}
