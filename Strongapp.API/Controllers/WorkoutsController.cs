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

        public WorkoutsController(ILogger<WorkoutsController> logger, WorkoutRepository repository, WorkoutService service, MeasurementRepository measurementsRepository)
        {
            _logger = logger;
            _repository = repository;
            _service = service;
            _measurementsRepository = measurementsRepository;
        }

        [HttpGet]
        public async Task<StrongWorkoutList> Get([FromQuery] int start, [FromQuery] int count)
        {
            var measurements = await _measurementsRepository.GetAsync();

            var items = _repository.AsQueryable()
                .OrderByDescending(x => x.Date)
                .Skip(start)
                .Take(count)
                .ToList()
                .Select(x => CreateWorkoutSummary(x, GetBodyweight(x.Date, measurements)))
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
            _service.UpdatePersonalRecords(workouts);
            foreach (var w in workouts) await _repository.UpdateAsync(w.Id, w);

            return Created($"/workouts/{workout.Id}", workout);
        }

        [HttpPut("{id}")]
        public async Task Put(string id, [FromBody] StrongWorkout workout)
        {
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

        private StrongWorkoutSummary CreateWorkoutSummary(StrongWorkout x, decimal bodyweight)
        {
            return new StrongWorkoutSummary
            {
                Id = x.Id,
                Date = x.Date,
                WorkoutName = x.WorkoutName,
                Volume = ComputeVolume(x, bodyweight),
                NumberOfPersonalRecords = x.ExerciseData.Sum(x => x.Sets.Sum(y => y.PersonalRecords.Count)),
                ExerciseData = x.ExerciseData.Select(y => new StrongExerciseDataSummary
                {
                    ExerciseName = y.ExerciseName,
                    Category = y.Category,
                    NumberOfSets = y.Sets.Count,
                    BestSet = Helpers.GetBestSet(y.Category, y.Sets)
                }).ToList()

            };
        }

        private decimal? ComputeVolume(StrongWorkout workout, decimal bodyweight)
        {
            decimal? volume = 0;
            foreach (var exerciseData in workout.ExerciseData)
            {
                volume += exerciseData.Sets.Sum(y => ComputeVolume(exerciseData.Category, bodyweight, y));
            }

            return volume;
        }

        private decimal? ComputeVolume(StrongExerciseCategory category, decimal bodyweight, StrongExerciseSetData set)
        {
            return category switch
            {
                StrongExerciseCategory.Barbell => set.Weight * set.Reps,
                StrongExerciseCategory.MachineOther => set.Weight * set.Reps,
                StrongExerciseCategory.Dumbbell => 2 * set.Weight * set.Reps,
                StrongExerciseCategory.WeightedBodyweight => (bodyweight + set.Weight) * set.Reps,
                StrongExerciseCategory.AssistedBodyweight => (bodyweight - set.Weight) * set.Reps,
                StrongExerciseCategory.RepsOnly => 0,
                StrongExerciseCategory.Cardio => 0,
                StrongExerciseCategory.Duration => 0,
                _ => 0
            };
        }

        private decimal GetBodyweight(DateTime date, List<StrongMeasurement> measurements)
        {
            var measurement =
                measurements
                    .Where(x => x.Name == "Weight")
                    .OrderBy(x => x.Date)
                    .Where(x => x.Date < date)
                    .LastOrDefault();

            decimal weight = 75;
            if (measurement != null)
            {
                weight = measurement.Value;
            }

            return weight;
        }
    }
}
