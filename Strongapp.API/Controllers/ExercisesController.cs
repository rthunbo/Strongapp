using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Strongapp.API.Repositories;
using Strongapp.Models;
using System.Security.Cryptography.Xml;
using Strongapp.API.Services;

namespace Strongapp.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ExercisesController : ControllerBase
    {
        private readonly ILogger<ExercisesController> _logger;
        private readonly WorkoutRepository _workoutRepository;
        private readonly ExerciseRepository _exerciseRepository;
        private readonly OneRMWeightCalculator _oneRmWeightCalculator;

        public ExercisesController(ILogger<ExercisesController> logger, WorkoutRepository repository, ExerciseRepository exerciseRepository, OneRMWeightCalculator oneRmWeightCalculator)
        {
            _logger = logger;
            _workoutRepository = repository;
            _exerciseRepository = exerciseRepository;
            _oneRmWeightCalculator = oneRmWeightCalculator;
        }

        [HttpGet("recordsHistory")]
        public async Task<StrongPersonalRecordsHistory> GetPersonalRecordsHistory([FromQuery] string name)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var exercise = exercises.First(x => x.ExerciseName == name);

            var workouts = await _workoutRepository.GetAsync();

            var performances = GetPerformances(name, workouts).ToList();

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                // ReSharper disable once InconsistentNaming
                var oneRMPr = performances
                    .Where(x => x.Set.HasOneRMPr)
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Weight = Convert.ToInt32(x.Set.OneRM) })
                    .OrderByDescending(x => x.Weight)
                    .ToList();
                var weightPr = performances
                    .Where(x => x.Set.HasWeightPr)
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Weight = x.Set.Weight, Reps = x.Set.Reps })
                    .OrderByDescending(x => x.Weight)
                    .ToList();
                var maxVolumePr = performances
                    .Where(x => x.Set.HasVolumePr)
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Weight = x.Set.Volume })
                    .OrderByDescending(x => x.Weight)
                    .ToList();

                return new StrongPersonalRecordsHistory
                {
                    OneRM = oneRMPr,
                    Weight = weightPr,
                    MaxVolume = maxVolumePr
                };

            }
            return null;
        }

        [HttpGet("records")]
        public async Task<StrongPersonalRecords> GetPersonalRecords([FromQuery] string name)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var exercise = exercises.First(x => x.ExerciseName == name);

            var workouts = await _workoutRepository.GetAsync();

            var performances = GetPerformances(name, workouts).ToList();

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                // ReSharper disable once InconsistentNaming
                var oneRMPr = performances
                    .Where(x => x.Set.HasOneRMPr)
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Weight = Convert.ToInt32(x.Set.OneRM) })
                    .LastOrDefault();
                var weightPr = performances
                    .Where(x => x.Set.HasWeightPr)
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Weight = x.Set.Weight, Reps = x.Set.Reps })
                    .LastOrDefault();
                var maxVolumePr = performances
                    .Where(x => x.Set.HasVolumePr)
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Weight = x.Set.Volume })
                    .LastOrDefault();

                var bestPerformances = GetBestPerformances(performances);
                var predictedPerformances = GetPredictedPerformances(bestPerformances, oneRMPr?.Weight);

                return new StrongPersonalRecords
                {
                    OneRM = oneRMPr,
                    Weight = weightPr,
                    MaxVolume = maxVolumePr,
                    PredictedPerformances = predictedPerformances
                };
            }

            return null;
        }

        private IEnumerable<StrongPerformance> GetPerformances(string name, List<StrongWorkout> workouts)
        {
            var matchedWorkouts = workouts
                .Where(x => x.ExerciseData.Any(x => x.ExerciseName == name));

            var performances = matchedWorkouts
                .Select(x =>
                    x.ExerciseData.First(x => x.ExerciseName == name).Sets.Select(y => new StrongPerformance
                        { Date = x.Date, Set = y }))
                .SelectMany(x => x);
            return performances;
        }

        private List<StrongBestPerformance> GetBestPerformances(IEnumerable<StrongPerformance> performances)
        {
            var bestPerformances = performances
                .GroupBy(x => x.Set.Reps)
                .Select(g =>
                {
                    var z = g.MaxBy(x => x.Set.Weight);
                    return new StrongBestPerformance { Date = z?.Date, Reps = g.Key, Weight = z?.Set.Weight };
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

        private List<StrongPredictedPerformance> GetPredictedPerformances(List<StrongBestPerformance> bestPerformances, decimal? oneRM)
        {
            var results = new List<StrongPredictedPerformance>();
            for (var reps = 1; reps <= 10; reps++)
            {
                var bestPerformance = bestPerformances.FirstOrDefault(x => x.Reps >= reps);
                var predictedWeight = _oneRmWeightCalculator.CalculatePredictedWeight(oneRM.Value, reps);
                results.Add(new StrongPredictedPerformance { Reps = reps, BestPerformance = bestPerformance, Predicted = Convert.ToInt32(predictedWeight) });
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
