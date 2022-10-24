using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Strongapp.API.Repositories;
using Strongapp.Models;
using System.Diagnostics.Metrics;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MeasurementsController : ControllerBase
    {
        private readonly ILogger<MeasurementsController> _logger;
        private readonly MeasurementRepository _repository;

        public MeasurementsController(ILogger<MeasurementsController> logger, MeasurementRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<StrongMeasurement>> Get()
        {
            return await _repository.GetAsync();
        }

        [HttpGet("{id}")]
        public async Task<StrongMeasurement> Get(string id)
        {
            return await _repository.GetAsync(id);
        }

        [HttpPost]
        public async Task Post([FromBody] StrongMeasurement workout)
        {
            await _repository.CreateAsync(workout);
        }

        [HttpPut("{id}")]
        public async Task Put(string id, [FromBody] StrongMeasurement workout)
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
