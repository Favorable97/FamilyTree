using FamilyTree.API.Errors;


namespace FamilyTree.API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

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
                });

                var response = ApiResponse<object>.Error("Ошибка валидации входных данных", ErrorCode.ValidationFailed, errors);

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (DomainException de)
            {
                context.Response.StatusCode = ErrorCodeHttpMapper.MapToStatusCode(de.ErrorCode);
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Error("Ошибка бизнесс логики", de.ErrorCode, de.Message);

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Error("Произошла критическая ошибка", ErrorCode.FatalError, ex.Message);

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
