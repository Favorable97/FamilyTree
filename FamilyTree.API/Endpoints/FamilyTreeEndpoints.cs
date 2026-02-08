namespace FamilyTree.API.Endpoints
{
    public static class FamilyTreeEndpoints
    {
        public static IEndpointRouteBuilder MapFamilyTreeEndponts(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/ft/api/relationship")
                .WithTags("FamilyTree");


            group.MapGet("/parents/{personId}", GetParents)
                .WithName("GetParentsByPerson")
                .WithSummary("Получить родителей персоны")
                .Produces<ApiResponse<List<ShortPersonDTO>>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);
            
            
            group.MapGet("/children/{personId}", GetChildren)
                .WithName("GetChildrenByPerson")
                .WithSummary("Получить детей персоны")
                .Produces<ApiResponse<List<ShortPersonDTO>>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            group.MapGet("/tree/{personId}", GetTreePerson)
                .WithName("GetTreeByPerson")
                .WithSummary("Получить дерево по персоне")
                .Produces<ApiResponse<PersonTreeNodeDTO>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status204NoContent)
                .Produces<ApiResponse<object>>(StatusCodes.Status500InternalServerError);

            return builder;
        } 

        private async static Task<IResult> GetParents(IFamilyTreeService service, Guid personId)
        {
            var (Mother, Father) = await service.GetParentsAsync(personId);

            List<ShortPersonDTO> parents = [];

            if (Mother != null)
                parents.Add(Mother);

            if (Father != null)
                parents.Add(Father);

            return parents.Count > 0 ? Results.Ok(ApiResponse<List<ShortPersonDTO>>.Ok(parents, "Родители персоны")) : Results.NoContent();
        }

        private async static Task<IResult> GetChildren(IFamilyTreeService service, Guid personId)
        {
            var data = await service.GetChildrenAsync(personId);

            return data.Count > 0 ? Results.Ok(ApiResponse<List<ShortPersonDTO>>.Ok(data, "Дети персоны")) : Results.NoContent();
        }

        private async static Task<IResult> GetTreePerson(IFamilyTreeService service, Guid personId, int maxDepth)
        {
            var tree = await service.GetPersonTreeAsync(personId, maxDepth, maxDepth);

            return tree != null ? Results.Ok(ApiResponse<PersonTreeNodeDTO>.Ok(tree, "Дерево")) : Results.NoContent();
        }
    }
}
