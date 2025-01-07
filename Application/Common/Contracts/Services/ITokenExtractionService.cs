

namespace Application.Common.Contracts.Services;

public interface ITokenExtractionService
{
    public string GetAccessFromHeader();
    public string GetRefreshFromCookie();
    public void SetRefreshToCookie(string refreshToken);
    public void RemoveRefreshFromCookie();
}
