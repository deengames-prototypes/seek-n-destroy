namespace SeekAndDestroy.Core.DataAccess
{
    public interface IUserRepository
    {
        /// <summary>
        /// Creates a user and returns the newly-created user's ID
        /// </summary>
        int CreateUser(string oauthId, string emailAddress);

        /// <summary>
        /// Get a user's ID. Probably returns zero if the user isn't in the DB.
        /// </summary>
        int GetUserId(string emailAddress);

        /// <summary>
        /// Increment all user's crystals depending on the number of crystal factories.
        /// </summary>
        void IncrementAllUserCrystals();
    }
}