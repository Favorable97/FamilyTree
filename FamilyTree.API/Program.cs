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
    var data = await service.GetAllPersonAsync();

    var response = ApiResponse<List<ShortPersonDTO>>.Ok(data, "");

    return data.Count > 0 ? Results.Ok(response) : Results.NotFound(response);
});

app.MapGet("/ft/api/persons/{id}", async (IPersonService service, Guid id) =>
{
    if (id == Guid.Empty)
        return Results.BadRequest("Указан некорректный идентификатор персоны!");

    var data = await service.GetPersonByIdAsync(id);

    var response = data == null ? ApiResponse<PersonDTO>.Ok(null, "Персона не найдена") : ApiResponse<PersonDTO>.Ok(data, "");

    return data != null ? Results.Ok(response) : Results.NotFound(response);
});

app.MapPost("/ft/api/persons", async (IPersonService service, IValidator<RequestAddPersonDTO> validator, RequestAddPersonDTO dto) =>
{
    var validate = await validator.ValidateAsync(dto);

    if (!validate.IsValid)
        throw new FluentValidation.ValidationException(validate.Errors);

    var data = await service.CreatePersonAsync(dto);

    var response = ApiResponse<PersonDTO>.Ok(data, "Персона успешно добавлена");

    return Results.Created($"/ft/api/persons/{data!.Id}", response);
});

app.MapPatch("/ft/api/persons/{id}", async (IPersonService service, IValidator<RequestUpdatePersonDTO> validator, Guid id, RequestUpdatePersonDTO dto) =>
{
    if (id == Guid.Empty)
        return Results.BadRequest("Указан некорректный идентификатор персоны!");

    var validate = await validator.ValidateAsync(dto);

    if (!validate.IsValid)
        throw new FluentValidation.ValidationException(validate.Errors);

    var data = await service.UpdatePersonAsync(id, dto);

    var response = ApiResponse<PersonDTO>.Ok(data, "Персона успешно обновлена!");

    return Results.Ok(response);
});

app.MapDelete("/ft/api/persons/{id}", async (IPersonService service, Guid id) =>
{
    await service.DeletePersonAsync(id);

    return Results.Ok(ApiResponse<object>.Ok(null, "Персона удалена из системы!"));
});

app.MapGet("/ft/api/test/{personId}", async (IFamilyTreeService service, Guid personId, int maxDepth) =>
{
    var result1 = await service.GetParentsAsync(personId);

    var result2 = await service.GetChildrenAsync(personId);

    var result5 = await service.GetAncestorsAsync(personId, maxDepth);

    var result6 = await service.GetDescendantsAsync(personId, maxDepth);
});

app.Run();

