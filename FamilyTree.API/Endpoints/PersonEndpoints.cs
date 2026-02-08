using FluentValidation;

namespace FamilyTree.API.Endpoints
{
    public static class PersonEndpoints
    {
        public static IEndpointRouteBuilder MapPersonEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("ft/api/persons");

            group.MapGet("/", GetAllPersons);
            group.MapGet("/{id}", GetPersonById);
            group.MapPost("/", CreatePerson);
            group.MapPatch("/{id}", UpdatePerson);
            group.MapDelete("/{id}", DeletePerson);

            return builder;
        }

        private async static Task<IResult> GetAllPersons(IPersonService service)
        {
            var data = await service.GetAllPersonAsync();

            var response = ApiResponse<List<ShortPersonDTO>>.Ok(data, "");

            return data.Count > 0 ? Results.Ok(response) : Results.NotFound(response);
        }

        private async static Task<IResult> GetPersonById(IPersonService service, Guid id)
        {
            if (id == Guid.Empty)
                return Results.BadRequest("Указан некорректный идентификатор персоны!");

            var data = await service.GetPersonByIdAsync(id);

            var response = data == null ? ApiResponse<PersonDTO>.Ok(null, "Персона не найдена") : ApiResponse<PersonDTO>.Ok(data, "");

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
                return Results.BadRequest("Указан некорректный идентификатор персоны!");

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
