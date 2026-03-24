
using FamilyTree.API.Validators;
using FamilyTree.Data.Context;
using FamilyTree.Data.Interfaces;
using FamilyTree.Data.Repositories;
using FluentValidation;
using FamilyTree.API.Services;
using System.Text.Json.Serialization;

namespace FamilyTree.API
{
    public static class ServiceProviderExtensions
    {
        public static void AddStartServices(this WebApplicationBuilder builder)
        {
            builder.Services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Добавляем сервис для подключения к БД через DI
            builder.Services.AddScoped<FamilyTreeContext>(_ => new FamilyTreeContext(builder.Configuration.GetConnectionString("mssql")!));

            builder.Services.AddScoped<IPersonService, PersonService>();

            builder.Services.AddScoped<IFamilyTreeService, FamilyTreeService>();
            
            builder.Services.AddScoped<IPersonRepository, PersonRepository>();

            builder.Services.AddScoped<IMarriageService, MarriageService>();

            builder.Services.AddScoped<IMarriageRepository, MarriageRepository>();

            builder.Services.AddScoped<ILifeEventService, LifeEventService>();

            builder.Services.AddScoped<ILifeEventRepository, LifeEventRepository>();

            // Достаточно объявления только этой строки, чтобы взялись все валидаторы из этой сборки, которые наследуются от AbstractValidator
            builder.Services.AddValidatorsFromAssemblyContaining<CreatePersonValidator>();

            // Добавление Swagger к проекту
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }
    }
}
