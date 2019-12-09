namespace SeekAndDestroy.Core.DataAccess
{
    public interface IResourcesRepository
    {
        void InitializeForUser(int userId);
    }
}