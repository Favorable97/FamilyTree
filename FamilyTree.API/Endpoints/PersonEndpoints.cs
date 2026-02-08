using FluentValidation;

namespace FamilyTree.API.Endpoints
{
    public static class PersonEndpoints
    {
        public static IEndpointRouteBuilder MapPersonEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("ft/api/persons")
                .WithTags("PersonService");

            group.MapGet("/", GetAllPersons)
                .WithName("GetAllPersons")
                .WithSummary("Получить всех людей из системы")
                .Produces<ApiResponse<List<ShortPersonDTO>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<List<ShortPersonDTO>>>(StatusCodes.Status404NotFound)
                .Produces<object>(StatusCodes.Status500InternalServerError);

            group.MapGet("/{id}", GetPersonById)
                .WithName("GetPersonById")
                .WithSummary("Получить человека по Id")
                .Produces<ApiResponse<ShortPersonDTO>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<ShortPersonDTO>>(StatusCodes.Status404NotFound)
                .Produces<object>(StatusCodes.Status500InternalServerError);

            group.MapPost("/", CreatePerson)
                .WithName("GeneratePerson")
                .WithSummary("Добавить человека")
                .Accepts<RequestAddPersonDTO>("application/json")
                .Produces<ApiResponse<PersonDTO>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity)
                .Produces<object>(StatusCodes.Status500InternalServerError);

            group.MapPatch("/{id}", UpdatePerson)
                .WithName("UpdatePerson")
                .WithSummary("Обновить человека (Частичное обновление)")
                .Accepts<RequestUpdatePersonDTO>("application/json")
                .Produces<ApiResponse<PersonDTO>>(StatusCodes.Status201Created)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity)
                .Produces<object>(StatusCodes.Status500InternalServerError);

            group.MapDelete("/{id}", DeletePerson)
                .WithName("DeletePerson")
                .Produces<ApiResponse<object>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<object>(StatusCodes.Status500InternalServerError);

            return builder;
        }

        private async static Task<IResult> GetAllPersons(IPersonService service)
        {
            var data = await service.GetAllPersonAsync();

            var response = data.Count > 0 
                ? ApiResponse<List<ShortPersonDTO>>.Ok(data!, "") 
                : ApiResponse<List<ShortPersonDTO>>.Error("Список пуст", Errors.ErrorCode.PersonNotFound, null);

            return data.Count > 0 ? Results.Ok(response) : Results.NotFound(response);
        }

        private async static Task<IResult> GetPersonById(IPersonService service, Guid id)
        {
            if (id == Guid.Empty)
                return Results.BadRequest("Указан некорректный идентификатор персоны!");

            var data = await service.GetPersonByIdAsync(id);

            var response = data == null 
                ? ApiResponse<PersonDTO>.Error("Персона не найдена", Errors.ErrorCode.PersonNotFound, null) 
                : ApiResponse<PersonDTO>.Ok(data, "");

            return data != null ? Results.Ok(response) : Results.NotFound(response);
        }

        private async static Task<IResult> CreatePerson(IPersonService service, IValidator<RequestAddPersonDTO> validator, RequestAddPersonDTO dto)
        {
            var validate = await validator.ValidateAsync(dto);

            if (!validate.IsValid)
                throw new FluentValidation.ValidationException(validate.Errors);

            var data = await service.CreatePersonAsync(dto);

            var response = ApiResponse<PersonDTO>.Ok(data, "Персона успешно добавлена");

            return Results.Created($"/ft/api/persons/{data!.Id}", response);
        }

        private async static Task<IResult> UpdatePerson(IPersonService service, IValidator<RequestUpdatePersonDTO> validator, Guid id, RequestUpdatePersonDTO dto)
        {
            if (id == Guid.Empty)
                return Results.BadRequest(ApiResponse<object>.Error("Указан некорректный идентификатор персоны!", Errors.ErrorCode.ValidationFailed, null));

            var validate = await validator.ValidateAsync(dto);

            if (!validate.IsValid)
                throw new FluentValidation.ValidationException(validate.Errors);

            var data = await service.UpdatePersonAsync(id, dto);

            var response = ApiResponse<PersonDTO>.Ok(data, "Персона успешно обновлена!");

            return Results.Ok(response);
        }

        private async static Task<IResult> DeletePerson(IPersonService service, Guid id)
        {
            await service.DeletePersonAsync(id);

            return Results.Ok(ApiResponse<object>.Ok(null, "Персона удалена из системы!"));
        }
    }
}
