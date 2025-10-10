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
                @"INSERT INTO Person (ID, LastName, FirstName, MiddleName, BirthDate, DeathDate, GenderID, MotherID, FatherID) 
                VALUES (@ID, @LastName, @FirstName, @MiddleName, @BirthDate, @DeathDate, @Gender, @MotherID, @FatherID)";

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
    }
}
