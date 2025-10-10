
using FamilyTree.Data.Context;
using FamilyTree.Data.Interfaces;
using FamilyTree.Data.Repositories;

namespace FamilyTree.API
{
    public static class ServiceProviderExtensions
    {
        public static void AddStartServices(this WebApplicationBuilder builder)
        {
            // Добавляем сервис для подключения к БД через DI
            builder.Services.AddScoped<FamilyTreeContext>(_ => new FamilyTreeContext(builder.Configuration.GetConnectionString("mssql")!));

            builder.Services.AddScoped<IPersonService, PersonService>();
            
            builder.Services.AddScoped<IPersonRepository, PersonRepository>();
        }
    }
}
