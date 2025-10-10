using FamilyTree.API;
using FamilyTree.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddStartServices();

var app = builder.Build();

app.ExceptionMiddleware();

app.MapPost("/ft/api/addPerson", async (IPersonService service, RequestAddPersonDTO dto) =>
{
    await service.AddPerson(dto);
});

app.Run();

