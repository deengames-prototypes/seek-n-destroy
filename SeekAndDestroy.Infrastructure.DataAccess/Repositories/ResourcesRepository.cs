using Npgsql;
using SeekAndDestroy.Core.DataAccess;
using Dapper;

namespace SeekAndDestroy.Infrastructure.DataAccess.Repositories
{
    public class ResourcesRepository : IResourcesRepository
    {
        private string _connectionString;

        public ResourcesRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public void InitializeForUser(int userId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute("INSERT INTO resources VALUES (@user_id, 0);", new { user_id = userId});
            }
        }
    }
}