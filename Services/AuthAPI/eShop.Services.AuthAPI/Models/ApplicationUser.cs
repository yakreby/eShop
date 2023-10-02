using Microsoft.AspNetCore.Identity;

namespace eShop.Services.AuthAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public string City { get; set; }
    }
}

