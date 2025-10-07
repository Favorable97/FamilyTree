namespace FamilyTree.API.Middleware
{
    public static class ExtensionsMiddleware
    {
        public static IApplicationBuilder ExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
