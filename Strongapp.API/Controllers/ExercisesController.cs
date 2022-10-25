using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Strongapp.API.Repositories;
using Strongapp.Models;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly ILogger<ExercisesController> _logger;
        private readonly WorkoutRepository _workoutRepository;
        private readonly ExerciseRepository _exerciseRepository;

        public ExercisesController(ILogger<ExercisesController> logger, WorkoutRepository repository, ExerciseRepository exerciseRepository)
        {
            _logger = logger;
            _workoutRepository = repository;
            _exerciseRepository = exerciseRepository;
        }

        [HttpGet("history")]
        public async Task<StrongExerciseDataHistoryList> GetHistory([FromQuery] string name, [FromQuery] int start, [FromQuery] int count)
        {
            var workouts = await _workoutRepository.GetAsync();

            var matchedWorkouts = workouts
                .Where(x => x.ExerciseData.Any(x => x.ExerciseName == name));

            var items = matchedWorkouts
                .OrderByDescending(x => x.Date)
                .Skip(start)
                .Take(count)
                .Select(x => new StrongExerciseDataHistory
                {
                    Date = x.Date,
                    WorkoutName = x.WorkoutName,
                    Sets = x.ExerciseData.First(x => x.ExerciseName == name).Sets
                })
                .ToList();
            var totalCount = matchedWorkouts.Count();

            return new StrongExerciseDataHistoryList { Items = items, TotalItemCount = totalCount };
        }

        [HttpGet]
        public async Task<IEnumerable<StrongExerciseWithMetadata>> GetExercises()
        {
            var workouts = await _workoutRepository.GetAsync();
            var previousPerformances = workouts
                .SelectMany(x => x.ExerciseData)
                .GroupBy(x => x.ExerciseName)
                .Select(g => g.Last())
                .OrderBy(x => x.ExerciseName);

            var histories = workouts
                .SelectMany(x => x.ExerciseData)
                .GroupBy(x => x.ExerciseName)
                .Select(g => new { ExerciseName = g.Key, List = g.ToList() });

            var exercises = await _exerciseRepository.GetAsync();
            return exercises.Select(x => {
                var previousPerformance = previousPerformances.FirstOrDefault(pp => pp.ExerciseName == x.ExerciseName);
                var history = histories.FirstOrDefault(h => h.ExerciseName == x.ExerciseName).List;
                return new StrongExerciseWithMetadata
                {
                    Id = x.Id,
                    BodyPart = x.BodyPart,
                    Category = x.Category,
                    ExerciseName = x.ExerciseName,
                    PreviousPerformance = previousPerformance,
                    BestSet = Helpers.GetBestSet(x.Category, history.Select(x => Helpers.GetBestSet(x.Category, x.Sets)))
                };
            });
        }
    }
}
