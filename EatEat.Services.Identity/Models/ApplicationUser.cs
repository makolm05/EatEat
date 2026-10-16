using Microsoft.AspNetCore.Identity;

namespace EatEat.Services.IdentityAPI.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
    }
}
