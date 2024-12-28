
using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests
{
    public class SignUpRequest
    {

        [EmailAddress]
        public string Email { get; set; } = null!;
        public  string FirstName { get; set; }=null!;
        public  string Password { get; set; } = null!;
    }
}
