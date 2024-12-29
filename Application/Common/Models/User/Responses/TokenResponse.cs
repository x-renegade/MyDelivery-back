using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Responses
{
    public abstract class TokenResponse
    {
        [Required(ErrorMessage = "AccessToken is required")]
        public string AccessToken { get; set; } = null!;
        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; } = null!;
    }
}
