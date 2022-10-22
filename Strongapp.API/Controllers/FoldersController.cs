using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Strongapp.API.Repositories;
using Strongapp.Models;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FoldersController : ControllerBase
    {
        private readonly ILogger<FoldersController> _logger;
        private readonly FolderRepository _repository;

        public FoldersController(ILogger<FoldersController> logger, FolderRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<StrongFolder>> Get()
        {
            return await _repository.GetAsync();
        }

        [HttpGet("{id}")]
        public async Task<StrongFolder> Get(string id)
        {
            return await _repository.GetAsync(id);
        }

        [HttpPost]
        public async Task Post([FromBody] StrongFolder workout)
        {
            await _repository.CreateAsync(workout);
        }

        [HttpPut("{id}")]
        public async Task Put(string id, [FromBody] StrongFolder workout)
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
