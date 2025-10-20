using FamilyTree.API;
using FamilyTree.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddStartServices();

var app = builder.Build();

app.ExceptionMiddleware();

app.MapGet("/ft/api/persons", async (IPersonService service) =>
{
    var result = await service.GetAllPersonAsync();

    Results.Ok(result);
});

app.MapGet("/ft/api/persons/{id}", async (IPersonService service, Guid id) =>
{
    var result = await service.GetPersonByIdAsync(id);

    Results.Ok(result);
});

app.MapPost("/ft/api/persons", async (IPersonService service, RequestAddPersonDTO data) =>
{
    await service.AddPersonAsync(data);
});

app.MapPost("/ft/api/persons/{childId}/setParent", async (IPersonService service, Guid childId, RequestSetParentDTO data) =>
{
    await service.SetParentAsync(childId, data);
});

app.MapDelete("/ft/api/persons/{id}", async (IPersonService service, Guid id) =>
{
    await service.DeletePersonAsync(id);
});

app.Run();

