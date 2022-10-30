using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Strongapp.API.Repositories;
using Strongapp.Models;
using System.Security.Cryptography.Xml;
using Strongapp.API.Services;
using static Strongapp.Models.StrongPersonalRecordType;
using System.Collections.Generic;
using System.Xml.Linq;

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
        private readonly ExerciseService _exerciseService;

        public ExercisesController(ILogger<ExercisesController> logger, WorkoutRepository repository, ExerciseRepository exerciseRepository, OneRMWeightCalculator oneRmWeightCalculator, ExerciseService exerciseService)
        {
            _logger = logger;
            _workoutRepository = repository;
            _exerciseRepository = exerciseRepository;
            _oneRmWeightCalculator = oneRmWeightCalculator;
            _exerciseService = exerciseService;
        }

        [HttpGet("recordsHistory")]
        public async Task<List<StrongPersonalRecord>> GetPersonalRecordsHistory([FromQuery] string name)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var exercise = exercises.First(x => x.ExerciseName == name);

            var workouts = await _workoutRepository.GetAsync();

            var performances = _exerciseService.GetPerformances(workouts, name);

            var personalRecords = new List<StrongPersonalRecord>();

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

            var maxRepsPr = performances
                .Where(x => x.Set.PersonalRecords.Contains(MaxReps))
                .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxReps, Reps = x.Set.Reps });
            personalRecords.AddRange(maxRepsPr);

            var maxWeightPr = performances
                .Where(x => x.Set.PersonalRecords.Contains(MaxWeightAdded))
                .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxWeightAdded, Weight = x.Set.Weight, Reps = x.Set.Reps });
            personalRecords.AddRange(maxWeightPr);

            var maxVolumeAddedPr = performances
                .Where(x => x.Set.PersonalRecords.Contains(MaxVolumeAdded))
                .Select(x => new StrongPersonalRecord { Date = x.Date, Type = MaxVolumeAdded, Weight = x.Set.Weight * x.Set.Reps });
            personalRecords.AddRange(maxVolumeAddedPr);

            return personalRecords.OrderBy(x => x.Date).ToList();
        }

        [HttpGet("records")]
        public async Task<List<StrongPersonalRecord>> GetPersonalRecords([FromQuery] string name)
        {
            var personalRecordsHistory = await GetPersonalRecordsHistory(name);

            return personalRecordsHistory
                .GroupBy(x => x.Type)
                .Select(g => g.Last())
                .ToList();
        }

        [HttpGet("predictedPerformances")]
        public async Task<List<StrongPredictedPerformance>> GetPredictedPerformances([FromQuery] string name)
        {
            var exercises = await _exerciseRepository.GetAsync();
            var exercise = exercises.First(x => x.ExerciseName == name);

            var workouts = await _workoutRepository.GetAsync();

            var performances = _exerciseService.GetPerformances(workouts, name);

            var result = new List<StrongPredictedPerformance>();

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                var bestPerformances = _exerciseService.GetBestPerformances(performances);
                var predictedPerformances = _exerciseService.GetPredictedPerformances(bestPerformances);
                result.AddRange(predictedPerformances);
            }

            return result;
        }

        [HttpGet("history")]
        public async Task<StrongHistoricExerciseDataList> GetHistory([FromQuery] string name, [FromQuery] int start, [FromQuery] int count)
        {
            var workouts = await _workoutRepository.GetAsync();

            var historicExerciseData = _exerciseService.GetHistoricExerciseData(workouts, name);
            var items = historicExerciseData
                .OrderByDescending(x => x.Date)
                .Skip(start)
                .Take(count)
                .ToList();
            var totalCount = historicExerciseData.Count();

            return new StrongHistoricExerciseDataList { Items = items, TotalItemCount = totalCount };
        }

        [HttpGet]
        public async Task<IEnumerable<StrongExerciseWithMetadata>> GetExercises()
        {
            var workouts = await _workoutRepository.GetAsync();

            var exercises = await _exerciseRepository.GetAsync();
            return exercises.Select(x => {
                var historicExerciseData = _exerciseService.GetHistoricExerciseData(workouts, x.ExerciseName);
                return new StrongExerciseWithMetadata
                {
                    Id = x.Id,
                    BodyPart = x.BodyPart,
                    Category = x.Category,
                    ExerciseName = x.ExerciseName,
                    PreviousPerformance = historicExerciseData.LastOrDefault(),
                    BestSet = Helpers.GetBestSet(x.Category, historicExerciseData.Select(z => Helpers.GetBestSet(x.Category, z.Sets)))
                };
            });
        }
    }
}
