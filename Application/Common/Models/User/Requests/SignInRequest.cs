using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public class SignInRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public required string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; } = null!;
    }
}
