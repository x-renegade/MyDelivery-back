using Application.Common.Exceptions;
using System.Net;
namespace Api.Middlewares;


public class GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger) : IMiddleware
{

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            // Пропускаем запрос дальше
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unexpected error occurred.");

            // Обработка исключения
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Стандартное сообщение об ошибке
        var statusCode = HttpStatusCode.InternalServerError;
        var errorType = "UnknownError";
        var errorMessage = exception.Message;

        // Обработка специфичных исключений
        switch (exception)
        {
            case InvalidTokenException:
                statusCode = HttpStatusCode.Unauthorized;
                errorType = "InvalidTokenException";
                errorMessage = "Your session has expired. Please log in again.";
                break;
            case UserNotFoundException:
                statusCode = HttpStatusCode.NotFound;
                errorType = "UserNotFoundException";
                errorMessage = "User does not exist.";
                break;
            case PasswordIncorrectException:
                statusCode = HttpStatusCode.BadRequest;
                errorType = "PasswordIncorrectException";
                errorMessage = "The password is incorrect.";
                break;
            case UserAlreadyExistsException:
                statusCode = HttpStatusCode.BadRequest;
                errorType = "UserAlreadyExistsException";
                errorMessage = exception.Message;
                break;
        }

        // Возвращаем ошибку в формате JSON
        context.Response.StatusCode = (int)statusCode;
        var result = new
        {
            error = errorMessage,
            type = errorType
        };

        return context.Response.WriteAsJsonAsync(result);
    }
}
