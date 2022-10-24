using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Strongapp.API.Repositories;
using Strongapp.Models;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkoutsController : ControllerBase
    {
        private readonly ILogger<WorkoutsController> _logger;
        private readonly WorkoutRepository _repository;

        public WorkoutsController(ILogger<WorkoutsController> logger, WorkoutRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IEnumerable<StrongWorkout>> Get()
        {
            return await _repository.GetAsync();
        }

        [HttpGet("paged")]
        public async Task<StrongWorkoutList> GetPaged([FromQuery] int start, [FromQuery] int count)
        {
            var items = _repository.AsQueryable()
                .OrderByDescending(x => x.Date)
                .Skip(start)
                .Take(count)
                .ToList();
            var totalCount = _repository.AsQueryable()
                .Count();

            return new StrongWorkoutList { Items = items, TotalItemCount = totalCount };
        }

        [HttpGet("{id}")]
        public async Task<StrongWorkout> Get(string id)
        {
            return await _repository.GetAsync(id);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StrongWorkout workout)
        {
            await _repository.CreateAsync(workout);
            return Created($"/workouts/{workout.Id}", workout);
        }

        [HttpPut("{id}")]
        public async Task Put(string id, [FromBody] StrongWorkout workout)
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
