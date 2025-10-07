using FamilyTree.Data.Context;

namespace FamilyTree.API
{
    public static class ServiceProviderExtensions
    {
        public static void AddStartServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<FamilyTreeContext>(_ => new FamilyTreeContext(builder.Configuration.GetConnectionString("mssql")!));
        }
    }
}
