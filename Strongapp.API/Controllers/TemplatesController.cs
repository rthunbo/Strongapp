using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Strongapp.API.Repositories;
using Strongapp.Models;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TemplatesController : ControllerBase
    {
        private readonly ILogger<TemplatesController> _logger;
        private readonly TemplateRepository _repository;

        public TemplatesController(ILogger<TemplatesController> logger, TemplateRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<StrongTemplate>> Get()
        {
            return await _repository.GetAsync();
        }

        [HttpGet("{id}")]
        public async Task<StrongTemplate> Get(string id)
        {
            return await _repository.GetAsync(id);
        }

        [HttpPost]
        public async Task Post([FromBody] StrongTemplate workout)
        {
            await _repository.CreateAsync(workout);
        }

        [HttpPut("{id}")]
        public async Task Put(string id, [FromBody] StrongTemplate workout)
        {
            await _repository.UpdateAsync(id, workout);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _repository.RemoveAsync(id);
        }
    }
}
