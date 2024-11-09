namespace Web.Extensions
{
    internal static class CorsExtension
    {
        internal static void ConfigureCors(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(name: "myCors",
                        policy =>
                        {
                            policy.WithOrigins(configuration["JWT:ValidAudience"]!)
                 .AllowAnyHeader()
                 .AllowAnyMethod()
                 .AllowCredentials();
                        });
            });
        }
    }
}
