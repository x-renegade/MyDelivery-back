using Application.Common.Models.User.Requests;
using Application.Common.Models.User.Responses;

namespace Application.Common.Contracts.Services;

public interface IAuthService
{
    Task<SignInResponse> SignInAsync(SignInRequest request);
    Task<bool> SignUpAsync(SignUpRequest request);
    Task<RefreshTokenResponse> RefreshTokenAsync();
    //Task RevokeAsync(User user);
    //Task RevokeAllAsync();
    Task SignOutAsync();
}
