
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public class SignUpRequest : BaseRequest
    {
        [Required(ErrorMessage = "FirstName is required")]
        public required string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; } = null!;
    }
}
