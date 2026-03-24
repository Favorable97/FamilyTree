using FamilyTree.Data.Context;

namespace FamilyTree.Data.Repositories
{
    public class LifeEventRepository(FamilyTreeContext context) : ILifeEventRepository
    {
        private readonly FamilyTreeContext _context = context;

        public async Task AddEventAsync(LifeEvent lifeEvent)
        {
            string sql =
                @"INSERT INTO LifeEvent(Id, PersonId, [Type], [Date], [Description])
                VALUES (@Id, @PersonID, @Type, @Date, @Description)";

            var parameters = ParametersParseSQLString.GetParamsFromCommand<LifeEvent>(sql, lifeEvent);

            await _context.ExecuteCommandAsync(sql, parameters);
        }

        public async Task<List<LifeEvent>> GetByPersonIdAsync(Guid personId)
        {
            string sql =
                @"SELECT 
                  Id,
                  PersonId,
                  [Type],
                  [Date],
                  [Description]
            FROM LifeEvent
            WHERE PersonId = @PersonID
            ORDER BY [Date]";

            var parameter = DBParameter.Create("@PersonID", personId);

            var data = await _context.QueryAsync(sql, parameter);

            return ConvertData.ConvertToListLifeEvent(data);
        }
    }
}
