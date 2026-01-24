using FamilyTree.API.Errors;

namespace FamilyTree.API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task<object?> InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                return null;
            }
            catch (DomainException de)
            {
                context.Response.StatusCode = 400;

                return ApiResponse<object>.Error(de.Message, de.ErrorCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());

                context.Response.StatusCode = 500;

                return ApiResponse<object>.Error(
                    "Произошла критическая ошибка!",
                    ErrorCode.FatalError
                );
            }
        }
    }
}
