using Microsoft.AspNetCore.Mvc;
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
        public async Task<IEnumerable<StrongExerciseData>> GetHistory([FromQuery] string name)
        {
            var workouts = await _workoutRepository.GetAsync();
            var exercisesHistory = workouts
                .SelectMany(x => x.ExerciseData)
                .Where(x => x.ExerciseName == name);
            return exercisesHistory;
        }

        [HttpGet]
        public async Task<StrongExercise> Get([FromQuery] string name)
        {
            return (await _exerciseRepository.GetAsync()).First(x => x.ExerciseName == name);
        }

        [HttpGet("all")]
        public async Task<IEnumerable<StrongExercise>> Get()
        {
            return await _exerciseRepository.GetAsync();
        }
    }
}
