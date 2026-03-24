namespace FamilyTree.API.Endpoints
{
    public static class LifeEventEndpoints
    {
        public static IEndpointRouteBuilder MapLifeEventEndpoints(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("ft/api/life-event")
                .WithTags("LifeEvent");

            group.MapGet("/{id}", GetTimeline)
                .WithName("GetTimeline")
                .WithSummary("Получить все события человека")
                .Produces<ApiResponse<List<LifeEventDTO>>>(StatusCodes.Status200OK)
                .Produces<ApiResponse<object>>(StatusCodes.Status404NotFound)
                .Produces<object>(StatusCodes.Status500InternalServerError);

            return group;
        }

        private async static Task<IResult> GetTimeline(ILifeEventService service, Guid id)
        {
            var timeline = await service.GetTimelineAsync(id);

            return Results.Ok(ApiResponse<List<LifeEventDTO>>.Ok(timeline, "События человека"));
        }
    }
}
