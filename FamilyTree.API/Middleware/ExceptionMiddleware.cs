namespace FamilyTree.API.Middleware
{
    public class ExceptionMiddleware(RequestDelegate next)
    {
        private RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
