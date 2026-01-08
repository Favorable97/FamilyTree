using FamilyTree.API;
using FamilyTree.API.Middleware;
using FamilyTree.API.Responses;
using FamilyTree.API.Validators;
using FluentValidation;

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
    if (id == Guid.Empty)
        return Results.BadRequest("”казан некорректный идентификатор персоны!");

    var result = await service.GetPersonByIdAsync(id);

    return result.Success ? Results.Ok(result) : Results.NotFound(result);
});

app.MapPost("/ft/api/persons", async (IPersonService service, IValidator<RequestAddPersonDTO> validator, RequestAddPersonDTO data) =>
{
    var validate = await validator.ValidateAsync(data);

    if (!validate.IsValid)
        return Results.BadRequest(ApiResponse<object>.Error(string.Join("; ", validate.Errors.Select(message => message.ErrorMessage))));

    var result = await service.CreatePersonAsync(data);

    return result.Success ? Results.Created($"/ft/api/persons/{result.Data!.Id}", result) : Results.BadRequest(result);
});

app.MapPatch("/ft/api/persons/{id}", async (IPersonService service, IValidator<RequestUpdatePersonDTO> validator, Guid id, RequestUpdatePersonDTO data) =>
{
    if (id == Guid.Empty)
        return Results.BadRequest("”казан некорректный идентификатор персоны!");

    var validate = await validator.ValidateAsync(data);

    if (!validate.IsValid)
        return Results.BadRequest(ApiResponse<object>.Error(string.Join("; ", validate.Errors.Select(message => message.ErrorMessage))));

    var result = await service.UpdatePersonAsync(id, data);

    return result.Success ? Results.Ok(result) : Results.NotFound(result);
});

app.MapDelete("/ft/api/persons/{id}", async (IPersonService service, Guid id) =>
{
    var result = await service.DeletePersonAsync(id);

    return result.Success ? Results.Ok(result) : Results.BadRequest(result);
});

app.MapGet("/ft/api/test/{personId}", async (IFamilyTreeService service, Guid personId, int maxDepth) =>
{
    var result1 = await service.GetParentsAsync(personId);

    var result2 = await service.GetChildrenAsync(personId);

    var result5 = await service.GetAncestorsAsync(personId, maxDepth);

    var result6 = await service.GetDescendantsAsync(personId, maxDepth);
});

app.Run();

