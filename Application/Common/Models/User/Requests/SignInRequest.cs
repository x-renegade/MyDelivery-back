using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public class SignInRequest
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; } = null!;
    }
}
