using Microsoft.OpenApi.Validations;
using Strongapp.API.Repositories;
using Strongapp.Models;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using static Strongapp.Models.StrongExerciseCategory;

namespace Strongapp.API.Services
{
    public class WorkoutService
    {
        private readonly MeasurementRepository _repository;
        private readonly OneRMWeightCalculator _oneRmWeightCalculator;

        public WorkoutService(MeasurementRepository repository, OneRMWeightCalculator oneRmWeightCalculator)
        {
            _repository = repository;
            _oneRmWeightCalculator = oneRmWeightCalculator;
        }

        public async Task UpdateVolume(StrongWorkout workout)
        {
            var bodyweight = await GetBodyweight(workout.Date);
            foreach (var exercise in workout.ExerciseData)
            {
                foreach (var set in exercise.Sets)
                {
                    set.Volume = ComputeVolume(exercise.Category, bodyweight, set);
                    if (exercise.Category == StrongExerciseCategory.WeightedBodyweight)
                    {
                        set.AddedVolume = set.Reps * set.Weight;
                    }
                }
            }
        }

        public async Task UpdateOneRM(StrongWorkout workout)
        {
            foreach (var exercise in workout.ExerciseData)
            {
                foreach (var set in exercise.Sets)
                {
                    set.OneRM = ComputeOneRM(exercise.Category, set);
                }
            }
        }

        public void UpdatePersonalRecords(List<StrongWorkout> workouts)
        {
            foreach (var workout in workouts)
            {
                var exerciseDataList = workouts
                    .Where(x => x.Date < workout.Date)
                    .SelectMany(x => x.ExerciseData)
                    .ToList();

                foreach (var exercise in workout.ExerciseData)
                {
                    var exerciseHistory = exerciseDataList
                        .Where(x => x.ExerciseName == exercise.ExerciseName);
                    
                    UpdatePersonalRecords(exercise, exerciseHistory);
                }
            }
        }

        private static void UpdatePersonalRecords(StrongExerciseData exercise, IEnumerable<StrongExerciseData> exerciseHistory)
        {
            var sets = exerciseHistory.SelectMany(x => x.Sets).ToList();

            foreach (var set in exercise.Sets)
            {
                set.PersonalRecords.Clear();
            }

            // Weight
            if (exercise.Category is MachineOther or Barbell or Dumbbell)
            {
                var personalRecord = sets.LastOrDefault(x => x.PersonalRecords.Contains(StrongPersonalRecordType.Weight));
                var maxWeightSet = exercise.Sets.MaxBy(x => x.Weight);
                if (maxWeightSet != null)
                {
                    if (personalRecord == null || maxWeightSet.Weight > personalRecord.Weight || (maxWeightSet.Weight == personalRecord.Weight && maxWeightSet.Reps > personalRecord.Reps))
                    {
                        maxWeightSet.PersonalRecords.Add(StrongPersonalRecordType.Weight);
                    }
                }
            }

            // MaxWeightAdded
            if (exercise.Category is WeightedBodyweight)
            {
                var personalRecord = sets.LastOrDefault(x => x.PersonalRecords.Contains(StrongPersonalRecordType.MaxWeightAdded));
                var maxWeightSet = exercise.Sets.MaxBy(x => x.Weight);
                if (maxWeightSet != null)
                {
                    if (personalRecord == null || maxWeightSet.Weight > personalRecord.Weight || (maxWeightSet.Weight == personalRecord.Weight && maxWeightSet.Reps > personalRecord.Reps))
                    {
                        maxWeightSet.PersonalRecords.Add(StrongPersonalRecordType.MaxWeightAdded);
                    }
                }
            }

            // MaxReps
            if (exercise.Category is RepsOnly or WeightedBodyweight or AssistedBodyweight)
            {
                var personalRecord = sets.LastOrDefault(x => x.PersonalRecords.Contains(StrongPersonalRecordType.MaxReps));
                var maxRepsSet = exercise.Sets.MaxBy(x => x.Reps);
                if (maxRepsSet != null)
                {
                    if (personalRecord == null || maxRepsSet.Reps > personalRecord.Reps)
                    {
                        maxRepsSet.PersonalRecords.Add(StrongPersonalRecordType.MaxReps);
                    }
                }
            }

            // MaxDuration
            if (exercise.Category is Duration or Cardio)
            {
                var personalRecord = sets.LastOrDefault(x => x.PersonalRecords.Contains(StrongPersonalRecordType.MaxDuration));
                var maxDurationSet = exercise.Sets.MaxBy(x => x.Seconds);
                if (maxDurationSet != null)
                {
                    if (personalRecord == null || maxDurationSet.Seconds > personalRecord.Seconds)
                    {
                        maxDurationSet.PersonalRecords.Add(StrongPersonalRecordType.MaxDuration);
                    }
                }
            }

            // MaxDistance
            if (exercise.Category is Cardio)
            {
                var personalRecord = sets.LastOrDefault(x => x.PersonalRecords.Contains(StrongPersonalRecordType.MaxDistance));
                var maxDistanceSet = exercise.Sets.MaxBy(x => x.Distance);
                if (maxDistanceSet != null)
                {
                    if (personalRecord == null || maxDistanceSet.Distance > personalRecord.Distance)
                    {
                        maxDistanceSet.PersonalRecords.Add(StrongPersonalRecordType.MaxDistance);
                    }
                }
            }

            // MaxVolume
            if (exercise.Category is MachineOther or Barbell or Dumbbell)
            {
                var personalRecord = sets.LastOrDefault(x => x.PersonalRecords.Contains(StrongPersonalRecordType.MaxVolume));
                var maxVolumeSet = exercise.Sets.MaxBy(x => x.Volume);
                if (maxVolumeSet != null)
                {
                    if (personalRecord == null || maxVolumeSet.Volume > personalRecord.Volume)
                    {
                        maxVolumeSet.PersonalRecords.Add(StrongPersonalRecordType.MaxVolume);
                    }
                }
            }

            // MaxVolumeAdded
            if (exercise.Category is WeightedBodyweight)
            {
                var personalRecord = sets.LastOrDefault(x => x.PersonalRecords.Contains(StrongPersonalRecordType.MaxVolumeAdded));
                var maxVolumeSet = exercise.Sets.MaxBy(x => x.AddedVolume);
                if (maxVolumeSet != null)
                {
                    if (personalRecord == null || maxVolumeSet.AddedVolume > personalRecord.AddedVolume)
                    {
                        maxVolumeSet.PersonalRecords.Add(StrongPersonalRecordType.MaxVolumeAdded);
                    }
                }
            }

            // OneRM
            if (exercise.Category is MachineOther or Barbell or Dumbbell)
            {
                var personalRecord = sets.LastOrDefault(x => x.PersonalRecords.Contains(StrongPersonalRecordType.OneRM));
                var maxOneRMSet = exercise.Sets.MaxBy(x => x.OneRM);
                if (maxOneRMSet != null)
                {
                    if (personalRecord == null || maxOneRMSet.OneRM > personalRecord.OneRM)
                    {
                        maxOneRMSet.PersonalRecords.Add(StrongPersonalRecordType.OneRM);
                    }
                }
            }

            // BestPace
            if (exercise.Category is Cardio)
            {
                // TODO 
            }
        }

        private async Task<decimal> GetBodyweight(DateTime date)
        {
            var measurements = await _repository.GetAsync();
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

        public decimal? ComputeVolume(StrongExerciseCategory category, decimal bodyweight, StrongExerciseSetData set)
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

        public decimal? ComputeOneRM(StrongExerciseCategory category, StrongExerciseSetData set)
        {
            if (category is StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell
                or StrongExerciseCategory.MachineOther)
            {
                return _oneRmWeightCalculator.CalculatePredictedOneRMWeight(set.Weight.Value, set.Reps.Value);
            }

            return null;
        }
    }
}
