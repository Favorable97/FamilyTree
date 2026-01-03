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
        private readonly FamilyTreeContext _context = context;

        public async Task CreatePersonAsync(Person person)
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

        public async Task<bool> ExistsAsync(
            string lastName,
            string firstName,
            string? middleName,
            DateTime dateBirthday)
        {
            string sql = @"
                SELECT 1 
                FROM Person 
                WHERE LastName = @LastName AND FirstName = @FirstName AND ISNULL(@MiddleName, '') = ISNULL(MiddleName, '') AND @DateBirthday = DateBirthdate";

            DBParameter[] parameters =
            [
                DBParameter.Create("@LastName", lastName),
                DBParameter.Create("@FirstName", firstName),
                DBParameter.Create("@MiddleName", middleName),
                DBParameter.Create("@DateBirthday", dateBirthday)
            ];

            var result = await _context.ExecuteScalarAsync(sql, parameters);

            return result != null;
        }

        public async Task UpdatePersonAsync(Person person)
        {
            string sql = @"
                UPDATE Person
                SET	LastName = @LastName,
	                FirstName = @FirstName,
	                MiddleName = @MiddleName,
	                BirthDate = @BirthDate,
	                DeathDate = @DeathDate,
	                Gender = @Gender,
	                MotherId = @MotherId,
	                FatherId = @FatherId
                WHERE Id = @Id";

            var parameters = ParametersParseSQLString.GetParamsFromCommand(sql, person);

            await _context.ExecuteCommandAsync(sql, parameters);
        }

        public async Task<bool> IsParentAsync(Guid id)
        {
            string sql = "SELECT 1 FROM Person WHERE @ID IN (FatherID, MotherID)";

            DBParameter parameter = DBParameter.Create("@ID", id);

            var result = await _context.ExecuteScalarAsync(sql, parameter);

            return result != null;
        }
    }
}
