using Microsoft.OpenApi.Validations;
using Strongapp.API.Repositories;
using Strongapp.Models;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;

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

            var weightPr = sets.LastOrDefault(x => x.HasWeightPr);
            var repsPr = sets.LastOrDefault(x => x.HasRepsPr);
            var durationPr = sets.LastOrDefault(x => x.HasDurationPr);
            var distancePr = sets.LastOrDefault(x => x.HasDistancePr);
            var volumePr = sets.LastOrDefault(x => x.HasVolumePr);
            // ReSharper disable once InconsistentNaming
            var oneRMPr = sets.LastOrDefault(x => x.HasOneRMPr);

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell or StrongExerciseCategory.WeightedBodyweight)
            {
                var maxWeightSet = exercise.Sets.MaxBy(x => x.Weight);
                if (maxWeightSet != null)
                {
                    maxWeightSet.HasWeightPr = weightPr == null || maxWeightSet.Weight > weightPr.Weight || (maxWeightSet.Weight == weightPr.Weight && maxWeightSet.Reps > weightPr.Reps);
                }
            }

            if (exercise.Category is StrongExerciseCategory.RepsOnly or StrongExerciseCategory.WeightedBodyweight or StrongExerciseCategory.AssistedBodyweight)
            {
                var maxRepsSet = exercise.Sets.MaxBy(x => x.Reps);
                if (maxRepsSet != null)
                {
                    maxRepsSet.HasRepsPr = repsPr == null || maxRepsSet.Reps > repsPr.Reps;
                }
            }

            if (exercise.Category is StrongExerciseCategory.Duration or StrongExerciseCategory.Cardio)
            {
                var maxDurationSet = exercise.Sets.MaxBy(x => x.Seconds);
                if (maxDurationSet != null)
                {
                    maxDurationSet.HasDurationPr = durationPr == null || maxDurationSet.Seconds > durationPr.Seconds;
                }
            }

            if (exercise.Category is StrongExerciseCategory.Cardio)
            {
                var maxDistanceSet = exercise.Sets.MaxBy(x => x.Distance);
                if (maxDistanceSet != null)
                {
                    maxDistanceSet.HasDistancePr = distancePr == null || maxDistanceSet.Distance > distancePr.Distance;
                }
            }

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                var maxVolumeSet = exercise.Sets.MaxBy(x => x.Volume);
                if (maxVolumeSet != null)
                {
                    maxVolumeSet.HasVolumePr = volumePr == null || maxVolumeSet.Volume > volumePr.Volume;
                }
            }

            if (exercise.Category is StrongExerciseCategory.WeightedBodyweight)
            {
                var maxVolumeSet = exercise.Sets.MaxBy(x => x.AddedVolume);
                if (maxVolumeSet != null)
                {
                    maxVolumeSet.HasVolumePr = volumePr == null || maxVolumeSet.AddedVolume > volumePr.AddedVolume;
                }
            }

            if (exercise.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell)
            {
                // ReSharper disable once InconsistentNaming
                var maxOneRMSet = exercise.Sets.MaxBy(x => x.OneRM);
                if (maxOneRMSet != null)
                {
                    maxOneRMSet.HasOneRMPr = oneRMPr == null || maxOneRMSet.OneRM > oneRMPr.OneRM;
                }
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
