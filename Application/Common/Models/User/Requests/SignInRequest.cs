using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public class SignInRequest : BaseRequest
    {
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = null!;
    }
}
