using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace FamilyTree.Data.Context
{
    public class FamilyTreeContext(string connectionString, ILogger<FamilyTreeContext> logger) : IAsyncDisposable
    {
        private readonly string _connectionString = connectionString;
        
        private readonly ILogger<FamilyTreeContext> _logger = logger;

        private SqlConnection? _connection;

        private SqlTransaction? _transaction;

        private async Task<SqlConnection> GetOpenConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
                await _connection.OpenAsync();
            }

            return _connection;
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
                throw new InvalidOperationException("Транзакция уже была открыта");

            var connection = await GetOpenConnection();

            _transaction = (SqlTransaction)await connection.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction == null)
                return;

            await _transaction.CommitAsync();

            await DisposeTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction == null)
                return;

            await _transaction.RollbackAsync();

            await DisposeTransactionAsync();
        }

        private async Task DisposeTransactionAsync()
        {
            await _transaction!.DisposeAsync();

            _transaction = null;
        }

        public async Task<int> ExecuteCommandAsync(string sqlCommand, params DBParameter[] parameters)
        {
            var connection = await GetOpenConnection();

            using var command = new SqlCommand(sqlCommand, connection);

            if (_transaction != null)
                command.Transaction = _transaction;
            
            foreach (var parameter in parameters)
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);

            int rows = await command.ExecuteNonQueryAsync();

            _logger.LogDebug("{Method}. Затронуто строк - {RowsCount}", nameof(ExecuteCommandAsync), rows);

            return rows;
        }

        public async Task<DataTable> QueryAsync(string sqlQuery, params DBParameter[] parameters)
        {
            var connection = await GetOpenConnection();

            using var command = new SqlCommand(sqlQuery, connection);

            if (_transaction != null)
                command.Transaction = _transaction;

            foreach (var parameter in parameters)
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);

            using var reader = await command.ExecuteReaderAsync();
            var resultTable = new DataTable();
            resultTable.Load(reader); // этот метод синхронный, но не блокирует БД. Только копирование данных

            _logger.LogDebug("{Method}. Количество строк: {RowsCount}", nameof(QueryAsync), resultTable.Rows.Count);

            return resultTable;
        }

        public async Task<object?> ExecuteScalarAsync(string sqlCommand, params DBParameter[] parameters)
        {
            var connection = await GetOpenConnection();

            using var command = new SqlCommand(sqlCommand, connection);

            if (_transaction != null)
                command.Transaction = _transaction;

            foreach (var parameter in parameters)
                command.Parameters.AddWithValue(parameter.Name, parameter.Value);

            var result = await command.ExecuteScalarAsync();

            _logger.LogDebug("{Method}. Результат был возвращен - {HasResult}", nameof(ExecuteScalarAsync), result != null);

            return result;
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction != null)
                await DisposeTransactionAsync();
            

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
