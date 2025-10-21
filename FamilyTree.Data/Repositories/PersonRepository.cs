using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Data.Context;

namespace FamilyTree.Data.Repositories
{
    public class PersonRepository(FamilyTreeContext context) : IPersonRepository
    {
        // Todo сделать обработку ошибок

        private readonly FamilyTreeContext _context = context;
        public async Task AddPersonAsync(Person person)
        {
            string sql =
                @"INSERT INTO Person (Id, LastName, FirstName, MiddleName, BirthDate, DeathDate, Gender, MotherId, FatherId) 
                VALUES (@Id, @LastName, @FirstName, @MiddleName, @BirthDate, @DeathDate, @Gender, @MotherId, @FatherId)";

            var parameters = ParametersParseSQLString.GetParamsFromCommand(sql, person);

            await _context.ExecuteCommandAsync(sql, parameters);
        }

        public async Task DeletePersonAsync(Guid id)
        {
            string sql = @"DELETE FROM Person WHERE Id = @ID";

            DBParameter parameter = DBParameter.Create("@ID", id);

            await _context.ExecuteCommandAsync(sql, parameter);
        }

        public async Task<List<Person>> GetAllPersonAsync()
        {
            string sql = @"SELECT * FROM Person";

            DataTable result = await _context.QueryAsync(sql);

            return ConvertData.ConvertToListPerson(result);
        }

        public async Task<Person?> GetPersonByIdAsync(Guid id)
        {
            string sql = @"SELECT * FROM Person WHERE Id = @ID";

            DBParameter parameter = DBParameter.Create("@ID", id);

            var result = await _context.QueryAsync(sql, parameter);

            return ConvertData.ConvertToListPerson(result).FirstOrDefault();
        }

        public Task UpdatePersonAsync(Person person)
        {
            throw new NotImplementedException();
        }

        public async Task SetParentAsync(Guid childId, Guid parentId, ParentRelation parentRelation)
        {
            string relation = parentRelation == ParentRelation.Mother ? "MotherId" : "FatherId";
            string sql = $"UPDATE Person SET {relation} = @ParentId WHERE ID = @Id";

            DBParameter[] parameters =
            [
                DBParameter.Create("@ParentId", parentId),
                DBParameter.Create("@Id", childId)
            ];

            await _context.ExecuteCommandAsync(sql, parameters);
        }
    }
}
