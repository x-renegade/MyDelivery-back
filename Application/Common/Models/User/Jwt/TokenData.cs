

using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Jwt;

public class TokenData
{
    [Required(ErrorMessage = "Email is required")]
    public  string Email { get; set; } = null!;
    [Required(ErrorMessage = "FirstName is required")]
    public  string FirstName { get; set; } = null!;
    [Required(ErrorMessage = "Id is required")]
    public  string Id { get; set; }=null!;
    [Required(ErrorMessage = "Jti is required")]
    public  string Jti { get; set; }=null!;
    [Required(ErrorMessage = "Role is required")]
    public  List<string> Roles { get; set; } = null!;
    [Required(ErrorMessage = "Exp is required")]
    public  string Exp { get; set; } = null!;
}
