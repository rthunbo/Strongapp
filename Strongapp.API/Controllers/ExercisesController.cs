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

        [HttpGet("search")]
        public async Task<IEnumerable<StrongExercise>> Search([FromQuery] string? searchPhrase)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var results = exercises;
            if (!string.IsNullOrEmpty(searchPhrase))
                results = results
                    .Where(x => x.ExerciseName.Contains(searchPhrase))
                    .ToList();
            return results;
        }

        [HttpGet("history")]
        public async Task<IEnumerable<StrongExerciseData>> GetHistory([FromQuery] string name)
        {
            var workouts = await _workoutRepository.GetAsync();
            var exercisesHistory = workouts
                .SelectMany(x => x.ExerciseData)
                .Where(x => x.ExerciseName == name);
            return exercisesHistory;
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

            var exercises = await _exerciseRepository.GetAsync();
            return exercises.Select(x => new StrongExerciseWithMetadata
            {
                Id = x.Id,
                BodyPart = x.BodyPart,
                Category = x.Category,
                ExerciseName = x.ExerciseName,
                PreviousPerformance = previousPerformances.FirstOrDefault(pp => pp.ExerciseName == x.ExerciseName),
           }).ToList();
        }
    }
}
