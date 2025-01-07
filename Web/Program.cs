using Api.Extensions;
using Api.Services.App;
namespace Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ��������� ��������
        builder.Services.ConfigureHosting(builder.Configuration);

        // ��������� URL
        builder.WebHost.UseUrls("https://localhost:5000");

        var app = builder.Build();

        // ��������� Middleware
        app.ConfigurePipeline();

        // ������������� �����
        await app.InitializeRolesAsync();

        app.Run();
    }
    
}
