namespace FamilyTree.API.Mappers
{
    public static class MarriageMapper
    {
        public static MarriageDTO MapToMarriageDTO(Marriage marriage, ShortPersonDTO spouse1, ShortPersonDTO spouse2) => new()
        {
            Id = marriage.Id,
            Spouse1 = spouse1,
            Spouse2 = spouse2,
            BeginDate = marriage.BeginDate,
            EndDate = marriage.EndDate,
            EndReason = marriage.EndReason
        };
            
    }
}
