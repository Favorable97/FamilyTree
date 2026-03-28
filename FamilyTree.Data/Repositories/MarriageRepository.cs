using FamilyTree.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FamilyTree.Data.Repositories
{
    public class MarriageRepository(FamilyTreeContext context) : IMarriageRepository
    {
        private readonly FamilyTreeContext _context = context;

        public async Task AddAsync(Marriage marriage)
        {
            string sql =
                @"INSERT INTO Marriage(Id, Spouse1Id, Spouse2Id, BeginDate, EndDate, EndReason)
                VALUES (@Id, @Spouse1Id, @Spouse2Id, @BeginDate, @EndDate, @EndReason)";

            var parameters = ParametersParseSQLString.GetParamsFromCommand<Marriage>(sql, marriage);

            await _context.ExecuteCommandAsync(sql, parameters);
        }

        public async Task UpdateAsync(Marriage marriage)
        {
            string sql =
                @"UPDATE Marriage
                SET 
                      Spouse1Id = @Spouse1Id,
                      Spouse2Id = @Spouse2Id,
                      BeginDate = @BeginDate,
                      EndDate = @EndDate,
                      EndReason = @EndReason
                WHERE Id = @Id";

            var parameters = ParametersParseSQLString.GetParamsFromCommand<Marriage>(sql, marriage);

            await _context.ExecuteCommandAsync(sql, parameters);
        }

        public async Task<Marriage?> GetActiveMarriageAsync(Guid personId)
        {
            string sql =
                @"SELECT *
                FROM Marriage
                WHERE @PersonID IN (Spouse1Id, Spouse2Id) AND EndDate IS NULL";

            DBParameter param = DBParameter.Create("@PersonID", personId);

            var data = await _context.QueryAsync(sql, param);

            return ConvertData.ConvertToListMarriage(data).FirstOrDefault();
        }

        public async Task<Marriage?> GetByIdAsync(Guid id)
        {
            string sql =
                @"SELECT *
                FROM Marriage
                WHERE Id = @Id";

            DBParameter param = DBParameter.Create("@Id", id);

            var data = await _context.QueryAsync(sql, param);

            return ConvertData.ConvertToListMarriage(data).FirstOrDefault();
        }

        public async Task<List<Marriage>> GetByPersonIdAsync(Guid personId)
        {
            string sql =
                @"SELECT *
                FROM Marriage
                WHERE @PersonId IN (Spouse1Id, Spouse2Id)
                ORDER BY BeginDate";

            DBParameter param = DBParameter.Create("@PersonId", personId);

            var data = await _context.QueryAsync(sql, param);

            return ConvertData.ConvertToListMarriage(data);
        }
    }
}
