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
            catch (DomainException de)
            {
                context.Response.StatusCode = ErrorCodeHttpMapper.MapToStatusCode(de.ErrorCode);
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Error(de.Message, de.ErrorCode);

                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var response = ApiResponse<object>.Error("Произошла критическая ошибка", ErrorCode.FatalError);

                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}
