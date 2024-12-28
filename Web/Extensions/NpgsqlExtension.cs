using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions
{
    internal static class NpgsqlExtension
    {
        internal static void CongigureContextNpgsql(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<Contex>(options => options.UseNpgsql(configuration.GetConnectionString("ConnStr")));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }
    }
}
