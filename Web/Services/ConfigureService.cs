using Api.Extensions;
using Api.Middlewares;
using Application.Common.Contracts;
using Application.Common.Contracts.Repositories;
using Application.Common.Contracts.Services;
using Application.Common.Utilities;
using Infrastructure.Repositories;
using Infrastructure.Services.Auth;

namespace Api.Services
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
            services.AddScoped<GlobalExceptionMiddleware>();
            services.AddScoped<IAppConfiguration, AppConfiguration>();
            services.AddScoped<IUserRepository, UserRepository>();


            services.AddLogging();
            services.AddControllers();


            services.ConfigureCors(configuration);
            services.CongigureContextNpgsql(configuration);
            services.ConfigureSwagger();
            services.ConfigureAuthentication(configuration);
            services.ConfigureIdentity();
        }
    }
}
