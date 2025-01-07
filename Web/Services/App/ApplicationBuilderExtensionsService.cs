using Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using System.Net.WebSockets;

namespace Api.Services.App
{
    public static class ApplicationBuilderExtensionsService
    {
        public static void ConfigurePipeline(this WebApplication app)
        {
            // Настройка Swagger
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Middleware для обработки ошибок
            app.UseMiddleware<GlobalExceptionMiddleware>();

            // HTTPS, CORS, маршруты и аутентификация
            app.UseWebSockets();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("myCors");
            app.UseAuthentication();
            app.UseAuthorization();

            // Настройка конечных точек
            app.MapControllers();
        }
    }
}
