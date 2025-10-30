using FamilyTree.API;
using FamilyTree.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.AddStartServices();

var app = builder.Build();

app.ExceptionMiddleware();

app.MapGet("/ft/api/persons", async (IPersonService service) =>
{
    var result = await service.GetAllPersonAsync();

    return result.Success ? Results.Ok(result) : Results.NotFound(result);
});

app.MapGet("/ft/api/persons/{id}", async (IPersonService service, Guid id) =>
{
    var result = await service.GetPersonByIdAsync(id);

    return result.Success ? Results.Ok(result) : Results.NotFound(result);
});

app.MapPost("/ft/api/persons", async (IPersonService service, RequestAddPersonDTO data) =>
{
    var result = await service.AddPersonAsync(data);

    return result.Success ? Results.Created($"/ft/api/persons/{result.Data!.Id}", result) : Results.BadRequest(result);
});

app.MapPatch("/ft/api/persons/{id}", async (IPersonService service, Guid id, RequestUpdatePersonDTO data) =>
{
    var result = await service.UpdatePersonAsync(id, data);

    return result.Success ? Results.Ok(result) : Results.NotFound(result);
});

app.MapDelete("/ft/api/persons/{id}", async (IPersonService service, Guid id) =>
{
    var result = await service.DeletePersonAsync(id);

    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

app.Run();

