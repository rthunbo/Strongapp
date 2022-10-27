using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Strongapp.API.Repositories;
using Strongapp.API.Services;
using Strongapp.Models;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class WorkoutsController : ControllerBase
    {
        private readonly ILogger<WorkoutsController> _logger;
        private readonly WorkoutRepository _repository;
        private readonly WorkoutService _service;

        public WorkoutsController(ILogger<WorkoutsController> logger, WorkoutRepository repository, WorkoutService service)
        {
            _logger = logger;
            _repository = repository;
            _service = service;
        }

        [HttpGet]
        public async Task<StrongWorkoutList> Get([FromQuery] int start, [FromQuery] int count)
        {
            var items = _repository.AsQueryable()
                .OrderByDescending(x => x.Date)
                .Skip(start)
                .Take(count)
                .ToList()
                .Select(x => new StrongWorkoutSummary
                {
                    Id = x.Id,
                    Date = x.Date,
                    WorkoutName = x.WorkoutName,
                    Volume = x.GetVolume(),
                    NumberOfPersonalRecords = x.GetNumberOfPrs(),
                    ExerciseData = x.ExerciseData.Select(y => new StrongExerciseDataSummary
                    {
                        ExerciseName = y.ExerciseName,
                        Category = y.Category,
                        NumberOfSets = y.Sets.Count,
                        BestSet = Helpers.GetBestSet(y.Category, y.Sets)
                    }).ToList()

                })
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
            await _service.UpdateVolume(workout);
            await _service.UpdateOneRM(workout);
            await _repository.CreateAsync(workout);

            var workouts = await _repository.GetAsync();
            _service.UpdatePersonalRecords(workouts);
            foreach (var w in workouts) await _repository.UpdateAsync(w.Id, w);

            return Created($"/workouts/{workout.Id}", workout);
        }

        [HttpPut("{id}")]
        public async Task Put(string id, [FromBody] StrongWorkout workout)
        {
            await _service.UpdateVolume(workout);
            await _service.UpdateOneRM(workout);
            await _repository.UpdateAsync(id, workout);

            var workouts = await _repository.GetAsync();
            _service.UpdatePersonalRecords(workouts);
            foreach (var w in workouts) await _repository.UpdateAsync(w.Id, w);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _repository.RemoveAsync(id);
        }
    }
}
