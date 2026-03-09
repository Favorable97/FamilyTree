using FamilyTree.API.Validators;
using FluentValidation;

namespace FamilyTree.API.Endpoints
{
    public static class MarriageEndpoints
    {
        public static IEndpointRouteBuilder MapMarriageEndponts(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/ft/api/marriage")
                .WithTags("Marriage");

            group.MapGet("/current-spouse", GetCurrentSpouse)
                .WithName("GetCurrentSpouse")
                .WithSummary("Получить текущего супруга")
                .Produces<ApiResponse<ShortPersonDTO?>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            group.MapGet("/history", GetMarriageHistory)
                .WithName("GetMarriageHistory")
                .WithSummary("Получить историю браков и разводов")
                .Produces<ApiResponse<List<MarriageDTO>>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            group.MapGet("/create", CreateMarriage)
                .WithName("CreateMarriage")
                .WithSummary("Создать брак")
                .Produces<ApiResponse<MarriageDTO>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
                .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
                .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            group.MapGet("/divorce", Divorce)
               .WithName("Divorce")
               .WithSummary("Добавление информации о разводе")
               .Produces<ApiResponse<MarriageDTO>>(StatusCodes.Status200OK)
               .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
               .Produces<ApiResponse<object>>(StatusCodes.Status400BadRequest)
               .Produces<ApiResponse<object>>(StatusCodes.Status409Conflict)
               .Produces<ApiResponse<object>>(StatusCodes.Status422UnprocessableEntity)
               .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            return group;
        }

        private async static Task<IResult> GetCurrentSpouse(IMarriageService service, Guid personId)
        {
            var spouse = await service.GetCurrentSpouseAsync(personId);

            return spouse == null 
                ? Results.NoContent() 
                : Results.Ok(ApiResponse<ShortPersonDTO>.Ok(spouse, "Текущий супруг"));
        }

        private async static Task<IResult> GetMarriageHistory(IMarriageService service, Guid personId)
        {
            var history = await service.GetMarriageHistoryAsync(personId);

            return history.Count == 0
                ? Results.NoContent()
                : Results.Ok(ApiResponse<List<MarriageDTO>>.Ok(history, "История браков и разводов"));
        }

        private async static Task<IResult> CreateMarriage(IMarriageService service, IValidator<RequestAddMarriageDTO> validator, RequestAddMarriageDTO dto)
        {
            var validate = await validator.ValidateAsync(dto);

            if (!validate.IsValid)
                throw new FluentValidation.ValidationException(validate.Errors);

            var marriage = await service.CreateMarriageAsync(dto);

            return Results.Ok(ApiResponse<MarriageDTO>.Ok(marriage, "Брак успешно добавлен"));
        }

        private async static Task<IResult> Divorce(IMarriageService service, RequestAddDivorceDTO dto)
        {
            var marriage = await service.DivorceAsync(dto);

            return Results.Ok(ApiResponse<MarriageDTO>.Ok(marriage, "Брак обновлен. Развод применен"));
        }
    }
}
