using FamilyTree.API;
using FamilyTree.API.Endpoints;
using FamilyTree.API.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.AddStartServices();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ExceptionMiddleware();

app.MapPersonEndpoints();

app.MapFamilyTreeEndponts();

app.MapMarriageEndponts();

app.MapLifeEventEndpoints();

app.Run();

