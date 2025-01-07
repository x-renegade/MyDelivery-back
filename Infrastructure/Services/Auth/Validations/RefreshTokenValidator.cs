using Application.Common.Contracts.Repositories;
using Application.Common.Contracts.Services;
using Application.Common.Exceptions;

namespace Infrastructure.Services.Auth.Validations;

public class RefreshTokenValidator(ITokenExtractionService tokenExtractionService, IUserRepository userRepository, ITokenService tokenService)
{
    public async Task<bool> RefreshTokensEqual()
    {
        var access = tokenExtractionService.GetAccessFromHeader();
        var data = tokenService.GetData(access);
        var user = await userRepository.GetUserByIdAsync(data.Id) ??
            throw new UserNotFoundException("User not found."); ;
        if (user.RefreshToken != tokenExtractionService.GetRefreshFromCookie())
            return false;
        return true;
    }
}
