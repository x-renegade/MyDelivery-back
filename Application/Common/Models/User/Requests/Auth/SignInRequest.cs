﻿using System.ComponentModel.DataAnnotations;

namespace Application.Common.Models.User.Requests.Auth
{
    public class SignInRequest : BaseRequest
    {
        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; } = null!;
    }
}
