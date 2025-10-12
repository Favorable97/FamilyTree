using FamilyTree.API;
using FamilyTree.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddStartServices();

var app = builder.Build();

app.ExceptionMiddleware();

app.MapGet("/ft/api/getAllPerson", async (IPersonService service) =>
{
    var result = await service.GetAllPersonAsync();

    Results.Ok(result);
});

app.MapPost("/ft/api/addPerson", async (IPersonService service, RequestAddPersonDTO data) =>
{
    await service.AddPerson(data);
});

app.MapPost("/ft/api/person/{childId}/setParent", async (IPersonService service, Guid childId, RequestSetParentDTO data) =>
{
    await service.SetParentAsync(childId, data);
});

app.MapDelete("/ft/api/person/{id}/deletePerson", async (IPersonService service, Guid id) =>
{
    await service.DeletePersonAsync(id);
});

app.Run();

