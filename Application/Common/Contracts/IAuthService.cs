using Application.Common.Models.User.Requests;
using Application.Common.Models.User.Responses;
using Domain.Entities;

namespace Application.Common.Contracts
{
    public interface IAuthService
    {
        Task<SignInResponse> SignIn(SignInRequest request);
        Task<bool> SignUp(SignUpRequest request);
        Task<RefreshTokenResponse> RefreshToken(RefreshTokenRequest request);
        Task Revoke(User user);
        Task RevokeAll();
    }
}
