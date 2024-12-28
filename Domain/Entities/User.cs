

using Microsoft.AspNetCore.Identity;

namespace Domain.Entities
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string? LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
