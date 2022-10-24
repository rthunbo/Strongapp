using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Strongapp.API.Repositories;
using Strongapp.Models;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AggregateController : ControllerBase
    {
        private readonly FolderRepository _folderRepository;
        private readonly TemplateRepository _templateRepository;

        public AggregateController(FolderRepository folderRepository, TemplateRepository templateRepository)
        {
            _folderRepository = folderRepository;
            _templateRepository = templateRepository;
        }

        [HttpGet]
        public async Task<StrongAggregateData> Get()
        {
            var folders = await _folderRepository.GetAsync();
            var templates = await _templateRepository.GetAsync();

            return new StrongAggregateData
            {
                Folders = folders,
                Templates = templates
            };
        }
    }
}
