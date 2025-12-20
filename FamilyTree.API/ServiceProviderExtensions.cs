
using FamilyTree.API.Validators;
using FamilyTree.Data.Context;
using FamilyTree.Data.Interfaces;
using FamilyTree.Data.Repositories;
using FluentValidation;

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

            // Достаточно объявления только этой строки, чтобы взялись все валидаторы из этой сборки, которые наследуются от AbstractValidator
            builder.Services.AddValidatorsFromAssemblyContaining<CreatePersonValidator>();
        }
    }
}
