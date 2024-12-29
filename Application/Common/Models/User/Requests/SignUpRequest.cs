
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public class SignUpRequest : BaseRequest
    {
        public string FirstName { get; set; } = null!;
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
