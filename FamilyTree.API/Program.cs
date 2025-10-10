using FamilyTree.API;
using FamilyTree.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddStartServices();

var app = builder.Build();

app.ExceptionMiddleware();

app.MapPost("/ft/api/addPerson", async (IPersonService service, RequestAddPersonDTO data) =>
{
    await service.AddPerson(data);
});

app.MapPost("/ft/api/person/{childId}/setParent", async (IPersonService service, Guid childId, RequestSetParentDTO data) =>
{
    await service.SetParentAsync(childId, data);
});

app.Run();

