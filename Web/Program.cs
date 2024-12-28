using Api.Extensions;
using Api.Middlewares;
using Api.Services;
using Application.Common.Contracts;
using Application.Common.Utilities;
using Application.Services;

namespace Apiæ
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigurationManager configuration = builder.Configuration;


            using var loggerFactory = LoggerFactory.Create(builder => { });
            // Add services to the container.

            builder.Services.ConfigureHosting(configuration);
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();
            builder.Services.AddScoped<GlobalExceptionMiddleware>();

            builder.Services.AddLogging();
            builder.Logging.AddConsole();

            builder.Services.AddControllers();

            builder.WebHost.UseUrls("https://localhost:5000");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("myCors");
            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            await UserRoleExtension.InitializeRolesAsync(app);
            app.MapControllers();

            app.Run();

        }
    }
}
