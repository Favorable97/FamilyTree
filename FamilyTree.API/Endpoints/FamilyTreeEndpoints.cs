namespace FamilyTree.API.Endpoints
{
    public static class FamilyTreeEndpoints
    {
        public static IEndpointRouteBuilder MapFamilyTreeEndponts(this IEndpointRouteBuilder builder)
        {
            var group = builder.MapGroup("/ft/api/relationship");


            group.MapGet("/parents/{personId}", GetParents);
            group.MapGet("/children/{personId}", GetChildren);
            group.MapGet("/tree/{personId}", GetTreePerson);

            return builder;
        } 

        private async static Task<IResult> GetParents(IFamilyTreeService service, Guid personId)
        {
            var data = await service.GetParentsAsync(personId);

            List<ShortPersonDTO> parents = [];

            if (data.Mother != null)
                parents.Add(data.Mother);

            if (data.Father != null)
                parents.Add(data.Father);

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

            return Results.Ok(ApiResponse<PersonTreeNodeDTO>.Ok(tree, "Дерево"));
        }
    }
}
