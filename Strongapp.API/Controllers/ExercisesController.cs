using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Strongapp.API.Repositories;
using Strongapp.Models;
using System.Security.Cryptography.Xml;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly ILogger<ExercisesController> _logger;
        private readonly WorkoutRepository _workoutRepository;
        private readonly ExerciseRepository _exerciseRepository;

        //                                      2      3      4      5      6      7      8      9      10
        decimal[] oneRm = new decimal[] { 0, 1, 0.95m, 0.93m, 0.90m, 0.87m, 0.85m, 0.83m, 0.80m, 0.77m, 0.75m };

        public ExercisesController(ILogger<ExercisesController> logger, WorkoutRepository repository, ExerciseRepository exerciseRepository)
        {
            _logger = logger;
            _workoutRepository = repository;
            _exerciseRepository = exerciseRepository;
        }

        [HttpGet("recordsHistory")]
        public async Task<StrongPersonalRecordsHistory> GetPersonalRecordsHistory([FromQuery] string name)
        {
            return null;
        }

        [HttpGet("records")]
        public async Task<StrongPersonalRecords> GetPersonalRecords([FromQuery] string name)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var exercise = exercises.First(x => x.ExerciseName == name);

            var workouts = await _workoutRepository.GetAsync();

            var predictedPerformances = new List<StrongPredictedPerformance>();

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                var performances = GetPerformances(name, workouts);
                var bestPerformances = GetBestPerformances(performances);

                predictedPerformances = GetPredictedPerformances(bestPerformances);
            }

            return new StrongPersonalRecords
            {
                PredictedPerformances = predictedPerformances
            };
        }

        private IEnumerable<StrongPerformance> GetPerformances(string name, List<StrongWorkout> workouts)
        {
            var matchedWorkouts = workouts
                .Where(x => x.ExerciseData.Any(x => x.ExerciseName == name));

            var performances = matchedWorkouts
                .Select(x =>
                    x.ExerciseData.First(x => x.ExerciseName == name).Sets.Select(y => new StrongPerformance
                        { Date = x.Date, Reps = y.Reps, Weight = y.Weight }))
                .SelectMany(x => x);
            return performances;
        }

        private List<StrongBestPerformance> GetBestPerformances(IEnumerable<StrongPerformance> performances)
        {
            var bestPerformances = performances
                .GroupBy(x => x.Reps)
                .Select(g =>
                {
                    var z = g.MaxBy(x => x.Weight);
                    return new StrongBestPerformance { Date = z?.Date, Reps = g.Key, Weight = z?.Weight };
                });

            var results = new List<StrongBestPerformance>();

            decimal weight = 0;
            foreach (var bestPerformance in bestPerformances.OrderByDescending(x => x.Reps))
            {
                if (bestPerformance.Weight.Value > weight)
                {
                    results.Add(bestPerformance);
                    weight = bestPerformance.Weight.Value;
                }
            }

            results = results.OrderBy(x => x.Reps).ToList();

            return results;
        }

        private List<StrongPredictedPerformance> GetPredictedPerformances(List<StrongBestPerformance> bestPerformances)
        {
            var maxOneRm = bestPerformances
                .Where(x => x.Reps <= 10)
                .Max(x => x.Weight / oneRm[x.Reps.Value]);

            var results = new List<StrongPredictedPerformance>();
            for (var reps = 1; reps <= 10; reps++)
            {
                var bestPerformance = bestPerformances.FirstOrDefault(x => x.Reps >= reps);
                results.Add(new StrongPredictedPerformance
                    { Reps = reps, BestPerformance = bestPerformance, Predicted = Convert.ToInt32(maxOneRm * oneRm[reps]) });
            }

            for (var reps = 11; reps <= 12; reps++)
            {
                var bestPerformance = bestPerformances.FirstOrDefault(x => x.Reps >= reps);
                results.Add(new StrongPredictedPerformance { Reps = reps, BestPerformance = bestPerformance });
            }

            return results;
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
