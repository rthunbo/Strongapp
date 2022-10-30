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
        private readonly MeasurementRepository _measurementsRepository;
        private readonly ExerciseRepository _exerciseRepository;

        public WorkoutsController(ILogger<WorkoutsController> logger, WorkoutRepository repository, WorkoutService service, MeasurementRepository measurementsRepository, ExerciseRepository exerciseRepository)
        {
            _logger = logger;
            _repository = repository;
            _service = service;
            _measurementsRepository = measurementsRepository;
            _exerciseRepository = exerciseRepository;
        }

        [HttpGet]
        public async Task<StrongWorkoutList> Get([FromQuery] int start, [FromQuery] int count)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var measurements = await _measurementsRepository.GetAsync();

            var items = _repository.AsQueryable()
                .OrderByDescending(x => x.Date)
                .Skip(start)
                .Take(count)
                .ToList()
                .Select(w => {
                    var bodyweight = Helpers.GetBodyweight(measurements, w.Date);
                    return new StrongWorkoutSummary
                    {
                        Id = w.Id,
                        Date = w.Date,
                        WorkoutName = w.WorkoutName,
                        Volume = w.ExerciseData.Sum(e => e.Sets.Sum(s =>
                        {
                            var exerciseCategory = exercises.First(x => x.ExerciseName == e.ExerciseName).Category;
                            return Helpers.ComputeVolume(exerciseCategory, bodyweight, s);
                        })),
                        NumberOfPersonalRecords = w.ExerciseData.Sum(e => e.Sets.Sum(s => s.PersonalRecords.Count)),
                        ExerciseData = w.ExerciseData.Select(e => 
                        {
                            var exerciseCategory = exercises.First(x => x.ExerciseName == e.ExerciseName).Category;
                            return new StrongExerciseDataSummary(e.ExerciseName, e.Sets.Count, Helpers.GetBestSet(exerciseCategory, e.Sets));
                        }).ToList()
                    };
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
            await _repository.CreateAsync(workout);

            var workouts = await _repository.GetAsync();
            await _service.UpdatePersonalRecords(workouts);
            foreach (var w in workouts) await _repository.UpdateAsync(w.Id, w);

            return Created($"/workouts/{workout.Id}", workout);
        }

        [HttpPut("{id}")]
        public async Task Put(string id, [FromBody] StrongWorkout workout)
        {
            await _repository.UpdateAsync(id, workout);

            var workouts = await _repository.GetAsync();
            await _service.UpdatePersonalRecords(workouts);
            foreach (var w in workouts) await _repository.UpdateAsync(w.Id, w);
        }

        [HttpDelete("{id}")]
        public async Task Delete(string id)
        {
            await _repository.RemoveAsync(id);
        }
    }
}
