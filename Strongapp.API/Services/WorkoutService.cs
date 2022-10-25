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

        public WorkoutService(MeasurementRepository repository)
        {
            _repository = repository;
        }

        public async Task UpdateVolume(StrongWorkout workout)
        {
            var bodyweight = await GetBodyweight(workout.Date);
            foreach (var exercise in workout.ExerciseData)
            {
                foreach (var set in exercise.Sets)
                {
                    set.Volume = ComputeVolume(exercise.Category, bodyweight, set);
                }
            }
        }

        public void UpdatePersonalRecords(List<StrongWorkout> workouts)
        {
            foreach (var workout in workouts)
            {
                foreach (var exercise in workout.ExerciseData)
                {
                    var exercisesHistory = workouts
                        .SelectMany(x => x.ExerciseData)
                        .Where(x => x.ExerciseName == exercise.ExerciseName && x.Date < workout.Date);
                    
                    UpdatePersonalRecords(exercise, exercisesHistory);
                }
            }
        }

        private static void UpdatePersonalRecords(StrongExerciseData exercise, IEnumerable<StrongExerciseData> exercisesHistory)
        {
            var sets = exercisesHistory.SelectMany(x => x.Sets).ToList();

            var weightPr = sets.LastOrDefault(x => x.HasWeightPr);
            
            var maxWeightSet = exercise.Sets.MaxBy(x => x.Weight);
            if (maxWeightSet != null)
            {
                maxWeightSet.HasWeightPr = false;
                if (weightPr == null || maxWeightSet.Weight > weightPr.Weight)
                {
                    maxWeightSet.HasWeightPr = true;
                }
            }

            var repsPr = sets.LastOrDefault(x => x.HasRepsPr);
            
            var maxRepsSet = exercise.Sets.MaxBy(x => x.Reps);
            if (maxRepsSet != null)
            {
                maxRepsSet.HasRepsPr = false;
                if (repsPr == null || maxRepsSet.Reps > repsPr.Reps)
                {
                    maxRepsSet.HasRepsPr = true;
                }
            }

            var durationPr = sets.LastOrDefault(x => x.HasDurationPr);
            
            var maxDurationSet = exercise.Sets.MaxBy(x => x.Seconds);
            if (maxDurationSet != null)
            {
                maxDurationSet.HasDurationPr = false;
                if (durationPr == null || maxDurationSet.Seconds > durationPr.Seconds)
                {
                    maxDurationSet.HasDurationPr = true;
                }
            }

            var distancePr = sets.LastOrDefault(x => x.HasDistancePr);
            
            var maxDistanceSet = exercise.Sets.MaxBy(x => x.Distance);
            if (maxDistanceSet != null)
            {
                maxDistanceSet.HasDistancePr = false;
                if (distancePr == null || maxDistanceSet.Distance > distancePr.Distance)
                {
                    maxDistanceSet.HasDistancePr = true;
                }
            }

            var volumePr = sets.LastOrDefault(x => x.HasVolumePr);
            
            var maxVolumeSet = exercise.Sets.MaxBy(x => x.Volume);
            if (maxVolumeSet != null)
            {
                maxVolumeSet.HasVolumePr = false;
                if (volumePr == null || maxVolumeSet.Volume > volumePr.Volume)
                {
                    maxVolumeSet.HasVolumePr = true;
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
    }
}
