using System.Net.Http.Json;
using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly HttpClient http;

        public TemplateService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<List<StrongTemplate>> GetTemplates()
        {
            var templates = await http.GetFromJsonAsync<List<StrongTemplate>>("templates");
            return templates;
        }

        public async Task<StrongTemplate> GetTemplateById(string id)
        {
            var template = await http.GetFromJsonAsync<StrongTemplate>($"templates/{id}");
            return template;
        }

        public async Task CreateTemplate(StrongTemplate template)
        {
            await http.PostAsJsonAsync<StrongTemplate>($"templates", template);
        }

        public async Task UpdateTemplate(string id, StrongTemplate template)
        {
            await http.PutAsJsonAsync<StrongTemplate>($"templates/{id}", template);
        }

        public async Task DeleteTemplate(string id)
        {
            await http.DeleteAsync($"templates/{id}");
        }
    }
}
