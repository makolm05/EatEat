using EatEat.Services.IdentityAPI.Models;

namespace EatEat.Services.IdentityAPI.Services.IServices
{
    public interface IJWTTokenGenerator
    {
        string GenerateToken(ApplicationUser user);
    }
}