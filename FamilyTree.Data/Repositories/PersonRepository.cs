using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FamilyTree.Data.Context;

namespace FamilyTree.Data.Repositories
{
    public class PersonRepository(FamilyTreeContext context) : IPersonRepository
    {
        private readonly FamilyTreeContext _context = context;
        public async Task AddPersonAsync(Person person)
        {
            string sql =
                @"INSERT INTO Person (Id, LastName, FirstName, MiddleName, BirthDate, DeathDate, GenderId, MotherId, FatherId) 
                VALUES (@Id, @LastName, @FirstName, @MiddleName, @BirthDate, @DeathDate, @Gender, @MotherId, @FatherId)";

            var parameters = ParametersParseSQLString.GetParamsFromCommand(sql, person);

            await _context.ExecuteCommandAsync(sql, parameters);
        }

        public Task DeletePersonAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<Person>> GetAllPersonAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Person?> GetPersonByIdAsync(Guid id)
        {
            throw new NotImplementedException();
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
