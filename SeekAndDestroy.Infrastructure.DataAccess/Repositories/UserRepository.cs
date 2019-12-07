using SeekAndDestroy.Core.DataAccess;
using Dapper;
using Npgsql;
using System.Collections.Generic;

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

            return this.GetUserId(emailAddress);
        }

        public int GetUserId(string emailAddress)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                return connection.ExecuteScalar<int>("SELECT user_id FROM users WHERE email_address = @emailAddress;", new { emailAddress });
            }
        }

        public void IncrementAllUserCrystals()
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                var rows = connection.Query("SELECT * FROM users");
                foreach(IDictionary<string, object> row in rows)
                {
                    var userId = row["user_id"];

                    var numFactories = connection.ExecuteScalar<int>("SELECT crystal_factories FROM buildings WHERE user_id = @userId", new { userId });
                    var currentCrystals = connection.ExecuteScalar<int>("SELECT crystals FROM resources WHERE user_id = @userId", new { userId });

                    connection.Execute("UPDATE users SET crystals = @newCrystals WHERE user_id = @userId", 
                        new { newCrystals=currentCrystals + numFactories, userId });
                }
            }
        }
    }
}