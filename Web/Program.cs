using Application.Common.Contracts;
using Application.Common.Utilities;
using Application.Services;
using Web.Extensions;
using Web.Middlewares;
using Web.Services;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            ConfigurationManager configuration = builder.Configuration;


            using var loggerFactory = LoggerFactory.Create(builder => { });
            // Add services to the container.

            builder.Services.ConfigureHosting(configuration);
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<ITokenService, TokenService>();


            builder.Services.AddControllers();

            builder.WebHost.UseUrls("http://localhost:5000");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<GlobalExceptionMiddleware>();
            app.ConfigureExceptionHandler(loggerFactory.CreateLogger("Exceptions"));
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("myCors");
            // Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
