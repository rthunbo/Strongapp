using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface ITemplateService
    {
        Task<List<StrongTemplate>> GetTemplates();

        Task<StrongTemplate> GetTemplateById(string id);

        Task CreateTemplate(StrongTemplate workout);

        Task UpdateTemplate(string id, StrongTemplate workout);

        Task DeleteTemplate(string id);
    }
}
