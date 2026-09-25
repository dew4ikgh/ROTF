using System.Net;
using System.Text.Json;
using ROTF.Server.Common;

namespace ROTF.Server.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Сталася непередбачувана помилка під час запиту до {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            string errorMessage = _env.IsDevelopment()
                ? $"На сервері сталася помилка: {exception.Message}"
                : "На сервері сталася внутрішня помилка. Будь ласка, спробуйте пізніше.";

            var response = ServiceResponse<string>.Fail(
                ServiceResultType.ValidationError,
                errorMessage
            );

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var jsonResult = JsonSerializer.Serialize(response, jsonOptions);
            await context.Response.WriteAsync(jsonResult);
        }
    }
}