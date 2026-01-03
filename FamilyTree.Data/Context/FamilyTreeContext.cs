using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;

namespace FamilyTree.Data.Context
{
    public class FamilyTreeContext(string connectionString) : IAsyncDisposable
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

        public async Task<int> ExecuteCommandAsync(string sqlCommand, params DBParameter[] parameters)
        {
            var connection = await GetOpenConnection();

            using var command = new SqlCommand(sqlCommand, connection);

            foreach (var parameter in parameters)
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);

            return await command.ExecuteNonQueryAsync();
        }

        public async Task<DataTable> QueryAsync(string sqlQuery, params DBParameter[] parameters)
        {
            var connection = await GetOpenConnection();

            using var command = new SqlCommand(sqlQuery, connection);

            foreach (var parameter in parameters)
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);

            using var reader = await command.ExecuteReaderAsync();
            var resultTable = new DataTable();
            resultTable.Load(reader); // этот метод синхронный, но не блокирует БД. Только копирование данных

            return resultTable;
        }

        public async Task<object?> ExecuteScalarAsync(string sqlCommand, params DBParameter[] parameters)
        {
            var connection = await GetOpenConnection();

            using var command = new SqlCommand(sqlCommand, connection);

            foreach (var parameter in parameters)
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);

            return await command.ExecuteScalarAsync();
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
                await _connection.DisposeAsync();
                _connection = null;
            }
            GC.SuppressFinalize(this);
        }
    }
}
