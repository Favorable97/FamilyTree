using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace FamilyTree.Data.Context
{
    public class FamilyTreeContext(string connectionString) : IDisposable
    {
        private readonly string _connectionString = connectionString;
        private SqlConnection? _connection;

        private async Task<SqlConnection> GetOpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
                await _connection.OpenAsync();
            }

            return _connection;
        }

        public async void Dispose()
        {
            await _connection.CloseAsync();
            await _connection.DisposeAsync();
            GC.SuppressFinalize(this);
        }
    }
}
