using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "AccessToken is required")]
        public required string AccessToken { get; set; } = null!;
        [Required(ErrorMessage = "RefreshToken is required")]
        public required string RefreshToken { get; set; } = null!;
        [Required(ErrorMessage = "Expiration is required")]
        public required DateTime Expiration { get; set; }
    }
}
