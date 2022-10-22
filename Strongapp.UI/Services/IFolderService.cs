using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IFolderService
    {
        Task<List<StrongFolder>> GetFolders();

        Task<StrongFolder> GetFolderById(string id);

        Task CreateFolder(StrongFolder workout);

        Task UpdateFolder(string id, StrongFolder workout);

        Task DeleteFolder(string id);
    }
}
