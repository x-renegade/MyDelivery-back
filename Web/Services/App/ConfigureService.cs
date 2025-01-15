using Api.Extensions;
using Api.Middlewares;
using Api.Validations;
using Application.Common.Contracts;
using Application.Common.Contracts.Repositories;
using Application.Common.Contracts.Services;
using Application.Common.Utilities;
using Infrastructure.Repositories.Location;
using Infrastructure.Repositories.User;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Auth.Validations;
using Infrastructure.Services.Location;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Api.Services.App
{
    public static class ConfigureService
    {
        public static void ConfigureHosting(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHttpContextAccessor();

            services.AddAuthentication();
            services.AddAuthorization();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IAppConfiguration, AppConfiguration>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenExtractionService, TokenExtractionService>();
            services.AddScoped<IAuthorizationFilter, AuthorizeTokenValidationFilter>();
            services.AddScoped<ILocationResository,LocationRepository>();
            services.AddScoped<ILocationService,LocationService>();
            services.AddScoped<RefreshTokenValidator>();


            services.AddLogging();
            services.AddControllers(options =>
            {
              //  options.Filters.Add<AuthorizeTokenValidationFilter>(); 
            });

            services.AddScoped<GlobalExceptionMiddleware>();

            services.ConfigureCors(configuration);
            services.CongigureContextNpgsql(configuration);
            services.ConfigureSwagger();
            services.ConfigureAuthentication(configuration);
            services.ConfigureIdentity();
        }
    }
}
