using Web.Extensions;
using Web.Middlewares;

namespace Web.Services
{
    public static class ConfigureService
    {
        public static void ConfigureHosting(this IServiceCollection services, ConfigurationManager configuration) {

            services.AddAuthentication();
            services.AddAuthorization();

            services.AddSingleton<GlobalExceptionMiddleware>();

            services.ConfigureCors(configuration);
            services.CongigureContextNpgsql(configuration);
            services.ConfigureSwagger();
            services.ConfigureAuthentication(configuration);
            services.ConfigureIdentity();
        }
    }
}
