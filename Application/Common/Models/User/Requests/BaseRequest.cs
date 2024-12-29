using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public abstract class BaseRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public required string Email { get; set; } = null!;
    }
}
