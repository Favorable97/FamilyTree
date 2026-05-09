using FamilyTree.API.Errors;


namespace FamilyTree.API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;

        private readonly ILogger<ExceptionMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (FluentValidation.ValidationException ve)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                context.Response.ContentType = "application/json";

                var errors = ve.Errors.Select(e => new
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                }).ToList();

                _logger.LogWarning(
                    "TraceId: {TraceId}. Не пройдена валидация для запроса {Method} {Path}. Errors: {@Errors}",
                    context.TraceIdentifier,
                    context.Request.Method,
                    context.Request.Path,
                    errors);

                var response = ApiResponse<object>.Error(
                    "Ошибка валидации входных данных", 
                    ErrorCode.ValidationFailed, 
                    context.TraceIdentifier,
                    errors);

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (DomainException de)
            {
                context.Response.StatusCode = ErrorCodeHttpMapper.MapToStatusCode(de.ErrorCode);
                context.Response.ContentType = "application/json";

                _logger.LogWarning(
                    "TraceId: {TraceId}. Доменная ошибка {ErrorCode} произошла во время обработки запроса в {Method} {Path}. Сообщение: {Message}",
                    context.TraceIdentifier,
                    de.ErrorCode,
                    context.Request.Method,
                    context.Request.Path,
                    de.Message);

                var response = ApiResponse<object>.Error(
                    "Ошибка бизнес-логики", 
                    de.ErrorCode, 
                    context.TraceIdentifier, 
                    null);

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                _logger.LogError(
                    ex,
                    "TraceId: {TraceId}. Произошла необработанная ошибка при обработке запроса {Method} {Path}.",
                    context.TraceIdentifier,
                    context.Request.Method,
                    context.Request.Path);

                var response = ApiResponse<object>.Error(
                    "Произошла необработанная ошибка", 
                    ErrorCode.FatalError, 
                    context.TraceIdentifier, 
                    null);

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
