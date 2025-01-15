using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Responses.Auth
{
    public abstract class TokenResponse
    {
        [Required(ErrorMessage = "AccessToken is required")]
        public required string AccessToken { get; set; } = null!;
        //[Required(ErrorMessage = "RefreshToken is required")]
        //public required string RefreshToken { get; set; } = null!;
    }
}
