using Npgsql;
using SeekAndDestroy.Core.DataAccess;
using Dapper;

namespace SeekAndDestroy.Infrastructure.DataAccess.Repositories
{
    public class BuildingsRepository : IBuildingsRepository
    {
        private string _connectionString;

        public BuildingsRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void InitializeForUser(int userId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute("INSERT INTO buildings VALUES (@user_id, @starting_crystal_factories);", new { user_id = userId, starting_crystal_factories = 1});
            }
        }
    }
}