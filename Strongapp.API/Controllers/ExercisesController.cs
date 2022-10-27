using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Strongapp.API.Repositories;
using Strongapp.Models;
using System.Security.Cryptography.Xml;
using Strongapp.API.Services;
using static Strongapp.Models.StrongPersonalRecordType;
using System.Collections.Generic;

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
        public async Task<List<StrongPersonalRecord>> GetPersonalRecordsHistory([FromQuery] string name)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var exercise = exercises.First(x => x.ExerciseName == name);

            var workouts = await _workoutRepository.GetAsync();
            var performances = GetPerformances(name, workouts).ToList();

            var personalRecords = new List<StrongPersonalRecord>();

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                var oneRMPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(OneRM))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = OneRM, 
                        Weight = Convert.ToInt32(_oneRmWeightCalculator.CalculatePredictedOneRMWeight(x.Set.Weight.Value, x.Set.Reps.Value)) });
                personalRecords.AddRange(oneRMPr);
                
                var weightPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(Weight))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = Weight, Weight = x.Set.Weight, Reps = x.Set.Reps });
                personalRecords.AddRange(weightPr);

                var volumeFactor = exercise.Category == StrongExerciseCategory.Dumbbell ? 2 : 1;
                var maxVolumePr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxVolume))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxVolume, Weight = volumeFactor * x.Set.Weight * x.Set.Reps });
                personalRecords.AddRange(maxVolumePr);
            }

            if (exercise.Category is StrongExerciseCategory.RepsOnly or StrongExerciseCategory.AssistedBodyweight)
            {
                var maxRepsPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxReps))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxReps, Reps = x.Set.Reps });
                personalRecords.AddRange(maxRepsPr);
            }

            if (exercise.Category is StrongExerciseCategory.WeightedBodyweight)
            {
                var maxRepsPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxReps))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxReps, Reps = x.Set.Reps });
                personalRecords.AddRange(maxRepsPr);

                var maxWeightPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxWeightAdded))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxWeightAdded, Weight = x.Set.Weight, Reps = x.Set.Reps });
                personalRecords.AddRange(maxWeightPr);

                var maxVolumePr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxVolumeAdded))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxVolumeAdded, Weight = x.Set.Weight * x.Set.Reps });
                personalRecords.AddRange(maxVolumePr);
            }

            return personalRecords.OrderBy(x => x.Date).ToList();
        }

        [HttpGet("records")]
        public async Task<List<StrongPersonalRecord>> GetPersonalRecords([FromQuery] string name)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var exercise = exercises.First(x => x.ExerciseName == name);

            var workouts = await _workoutRepository.GetAsync();

            var performances = GetPerformances(name, workouts).ToList();

            var personalRecords = new List<StrongPersonalRecord>();

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                var oneRMPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(OneRM))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = OneRM, 
                        Weight = Convert.ToInt32(_oneRmWeightCalculator.CalculatePredictedOneRMWeight(x.Set.Weight.Value, x.Set.Reps.Value)) })
                    .LastOrDefault();
                if (oneRMPr != null)
                {
                    personalRecords.Add(oneRMPr);
                }

                var weightPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(Weight))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = Weight, Weight = x.Set.Weight, Reps = x.Set.Reps })
                    .LastOrDefault();
                if (weightPr != null)
                {
                    personalRecords.Add(weightPr);
                }

                var volumeFactor = exercise.Category == StrongExerciseCategory.Dumbbell ? 2 : 1;
                var maxVolumePr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxVolume))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxVolume, Weight = volumeFactor * x.Set.Weight * x.Set.Reps })
                    .LastOrDefault();
                if (maxVolumePr != null)
                {
                    personalRecords.Add(maxVolumePr);
                }
            }

            if (exercise.Category is StrongExerciseCategory.RepsOnly or StrongExerciseCategory.AssistedBodyweight)
            {
                var maxRepsPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxReps))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxReps, Reps = x.Set.Reps })
                    .LastOrDefault();

                if (maxRepsPr != null)
                {
                    personalRecords.Add(maxRepsPr);
                }
            }

            if (exercise.Category is StrongExerciseCategory.WeightedBodyweight)
            {
                var maxRepsPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxReps))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxReps, Reps = x.Set.Reps })
                    .LastOrDefault();

                if (maxRepsPr != null)
                {
                    personalRecords.Add(maxRepsPr);
                }

                var maxWeightPr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxWeightAdded))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxWeightAdded, Weight = x.Set.Weight, Reps = x.Set.Reps })
                    .LastOrDefault();

                if (maxWeightPr != null)
                {
                    personalRecords.Add(maxWeightPr);
                }

                var maxVolumePr = performances
                    .Where(x => x.Set.PersonalRecords.Contains(MaxVolumeAdded))
                    .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxVolumeAdded, Weight = x.Set.Weight * x.Set.Reps })
                    .LastOrDefault();

                if (maxVolumePr != null)
                {
                    personalRecords.Add(maxVolumePr);
                }
            }

            return personalRecords;
        }

        [HttpGet("predictedPerformances")]
        public async Task<List<StrongPredictedPerformance>> GetPredictedPerformances([FromQuery] string name)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var exercise = exercises.First(x => x.ExerciseName == name);

            var workouts = await _workoutRepository.GetAsync();

            var performances = GetPerformances(name, workouts).ToList();

            var predictedPerformances = new List<StrongPredictedPerformance>();

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                var bestPerformances = GetBestPerformances(performances);
                predictedPerformances.AddRange(GetPredictedPerformances(bestPerformances));
            }

            return predictedPerformances;
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

        private List<StrongPredictedPerformance> GetPredictedPerformances(List<StrongBestPerformance> bestPerformances)
        {
            var oneRM = bestPerformances.Max(x => _oneRmWeightCalculator.CalculatePredictedOneRMWeight(x.Weight.Value, x.Reps.Value));

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
