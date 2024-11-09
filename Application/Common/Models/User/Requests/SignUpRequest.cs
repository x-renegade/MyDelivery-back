
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public class SignUpRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public required string UserName { get; set; } = null!;

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public required string Email { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; } = null!;
    }
}
