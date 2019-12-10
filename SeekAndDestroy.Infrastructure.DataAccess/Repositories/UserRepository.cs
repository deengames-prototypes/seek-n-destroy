using SeekAndDestroy.Core.DataAccess;
using Dapper;
using Npgsql;

namespace SeekAndDestroy.Infrastructure.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private string _connectionString;

        public UserRepository(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public int CreateUser(string oauthId, string emailAddress)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Execute("INSERT INTO users (oauth_id, email_address) VALUES (@oauthId, @emailAddress);", new { oauthId, emailAddress });
            }

            return this.GetUserId(oauthId);
        }

        public int GetUserId(string oauthId)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection.ExecuteScalar<int>("SELECT user_id FROM users WHERE oauth_id = @oauthId;", new { oauthId });
            }
        }

        // TODO: perhaps discuss this further. This doesn't belong here, as it touches three tables.
        // It should quite possibly move to the resources repository (since it's primarily about
        // incrementing resources), or optionally, should be extracted into three calls per-repository,
        // and then stitched together in a context somewhere. ¯\_(ツ)_/¯
        public void IncrementAllUsersResourcesBasedOnBuildings()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var rows = connection.Query<int>("SELECT user_id FROM users");
                foreach(var userId in rows)
                {
                    connection.Execute(@"
                    update resources set crystals = 
                        (select r.crystals + b.crystal_factories
                            from resources r join users u on r.user_id = u.user_id
                            join buildings b on b.user_id = u.user_id
                            where u.user_id = @userId);
                    ", new { userId });
                }
            }
        }
    }
}