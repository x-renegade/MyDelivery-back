using Api.Middlewares;
using Api.Validations;
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


            // HTTPS, CORS, маршруты и аутентификация
            app.UseWebSockets();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors("myCors");
            app.UseAuthentication();
            app.UseAuthorization();

            // Middleware для обработки ошибок
            app.UseMiddleware<GlobalExceptionMiddleware>();
            // Настройка конечных точек
            app.MapControllers();
        }
    }
}
