using Api.Extensions;
using Api.Services.App;
namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Настройка сервисов
        builder.Services.ConfigureHosting(builder.Configuration);

        // Настройка URL
        builder.WebHost.UseUrls("https://localhost:5000");

        var app = builder.Build();

        // Настройка Middleware
        app.ConfigurePipeline();

        // Инициализация ролей
        await app.InitializeRolesAsync();

        app.Run();
    }
    
}
