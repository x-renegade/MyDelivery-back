using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Services.Auth.Validations;

namespace Api.Validations;

public class AuthorizeTokenValidationFilter(RefreshTokenValidator refreshTokenValidator) : IAuthorizationFilter
{
    public async void OnAuthorization(AuthorizationFilterContext context)
    {
        
        // Проверка, если метод контроллера имеет атрибут [Authorize]
        var hasAuthorizeAttribute = context.ActionDescriptor.EndpointMetadata
            .Any(metadata => metadata is AuthorizeAttribute);

        if (!hasAuthorizeAttribute)
            return;  // Если атрибута нет, пропускаем

        await refreshTokenValidator.RefreshTokensEqual();
        
       
    }
}
