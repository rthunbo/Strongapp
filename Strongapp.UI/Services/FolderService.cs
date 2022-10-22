using System.Net.Http.Json;
using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public class FolderService : IFolderService
    {
        private readonly HttpClient http;

        public FolderService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<List<StrongFolder>> GetFolders()
        {
            var folders = await http.GetFromJsonAsync<List<StrongFolder>>("folders");
            return folders;
        }

        public async Task<StrongFolder> GetFolderById(string id)
        {
            var folder = await http.GetFromJsonAsync<StrongFolder>($"folders/{id}");
            return folder;
        }

        public async Task CreateFolder(StrongFolder folder)
        {
            await http.PostAsJsonAsync<StrongFolder>($"folders", folder);
        }

        public async Task UpdateFolder(string id, StrongFolder folder)
        {
            await http.PutAsJsonAsync<StrongFolder>($"folders/{id}", folder);
        }

        public async Task DeleteFolder(string id)
        {
            await http.DeleteAsync($"folders/{id}");
        }
    }
}
