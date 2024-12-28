using Application.Common.Models.User.Requests;
using Application.Common.Models.User.Responses;
using Domain.Entities;

namespace Application.Common.Contracts
{
    public interface IAuthService
    {
        Task<SignInResponse> SignInAsync(SignInRequest request);
        Task<bool> SignUpAsync(SignUpRequest request);
        Task<RefreshTokenResponse> RefreshTokenAsync(RefreshTokenRequest request);
        Task RevokeAsync(User user);
        Task RevokeAllAsync();
    }
}
