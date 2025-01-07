using Api.Extensions;
using Api.Middlewares;
using Api.Validations;
using Application.Common.Contracts;
using Application.Common.Contracts.Repositories;
using Application.Common.Contracts.Services;
using Application.Common.Utilities;
using Infrastructure.Repositories;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Auth.Validations;

namespace Api.Services.App
{
    public static class ConfigureService
    {
        public static void ConfigureHosting(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHttpContextAccessor();

            services.AddAuthentication();
            services.AddAuthorization();
            services.AddScoped<GlobalExceptionMiddleware>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAppConfiguration, AppConfiguration>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenExtractionService, TokenExtractionService>();
            services.AddScoped<RefreshTokenValidator>();


            services.AddLogging();
            services.AddControllers(options =>
            {
              //  options.Filters.Add<AuthorizeTokenValidationFilter>(); 
            });


            services.ConfigureCors(configuration);
            services.CongigureContextNpgsql(configuration);
            services.ConfigureSwagger();
            services.ConfigureAuthentication(configuration);
            services.ConfigureIdentity();
        }
    }
}
