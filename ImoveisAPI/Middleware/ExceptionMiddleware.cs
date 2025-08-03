using System.Net;
using System.Text.Json;

namespace ImoveisAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            HttpStatusCode status;
            string message = ex.Message;

            switch (ex)
            {
                case KeyNotFoundException:
                    status = HttpStatusCode.NotFound; // 404
                    break;
                case InvalidOperationException:
                    status = HttpStatusCode.BadRequest; // 400
                    break;
                default:
                    status = HttpStatusCode.InternalServerError; // 500
                    message = "Ocorreu um erro inesperado.";
                    break;
            }

            var response = new { error = message };
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
