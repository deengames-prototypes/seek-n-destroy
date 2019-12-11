using Npgsql;
using SeekAndDestroy.Core.DataAccess;
using Dapper;
using System.Collections.Generic;
using SeekAndDestroy.Core.Enums;

namespace SeekAndDestroy.Infrastructure.DataAccess.Repositories
{
    public class ResourcesRepository : IResourcesRepository
    {
        private string _connectionString;

        public ResourcesRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public Dictionary<ResourceType, int> GetResources(int userId)
        {
            var toReturn = new Dictionary<ResourceType, int>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var reader = connection.ExecuteReader("SELECT * FROM resources WHERE user_id = @userId", new { userId });
                
                // Always expect back one row
                reader.Read();
                
                toReturn[ResourceType.Crystals] = int.Parse(reader["crystals"].ToString());
            }
            
            return toReturn;
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