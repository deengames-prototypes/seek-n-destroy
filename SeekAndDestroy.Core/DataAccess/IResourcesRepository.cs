using System.Collections.Generic;
using SeekAndDestroy.Core.Enums;

namespace SeekAndDestroy.Core.DataAccess
{
    public interface IResourcesRepository
    {
        void InitializeForUser(int userId);
        Dictionary<ResourceType, int> GetResources(int userId);
    }
}