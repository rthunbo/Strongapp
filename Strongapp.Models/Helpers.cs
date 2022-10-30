using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class Helpers
    {
        public static StrongExerciseSetData? GetBestSet(StrongExerciseCategory exerciseCategory, IEnumerable<StrongExerciseSetData?> sets)
        {
            return exerciseCategory switch
            {
                StrongExerciseCategory.AssistedBodyweight => sets
                    .OrderBy(x => x.Reps)
                    .ThenByDescending(x => x.Weight)
                    .LastOrDefault(),
                StrongExerciseCategory.WeightedBodyweight => sets
                    .OrderBy(x => x.Reps)
                    .ThenBy(x => x.Weight)
                    .LastOrDefault(),
                StrongExerciseCategory.RepsOnly => sets.MaxBy(x => x.Reps),
                StrongExerciseCategory.Duration => sets.MaxBy(x => x.Seconds),
                StrongExerciseCategory.Barbell => sets.MaxBy(x => x.Weight),
                StrongExerciseCategory.Dumbbell => sets.MaxBy(x => x.Weight),
                StrongExerciseCategory.MachineOther => sets.MaxBy(x => x.Weight),
                StrongExerciseCategory.Cardio => sets.MaxBy(x => x.Distance),
                _ => null
            };
        }

        public static decimal GetBodyweight(List<StrongMeasurement> measurements, DateTime date)
        {
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

        public static decimal ComputeVolume(StrongExerciseCategory category, decimal bodyweight, StrongExerciseSetData set)
        {
            return category switch
            {
                StrongExerciseCategory.Barbell => set.Weight.Value * set.Reps.Value,
                StrongExerciseCategory.MachineOther => set.Weight.Value * set.Reps.Value,
                StrongExerciseCategory.Dumbbell => 2 * set.Weight.Value * set.Reps.Value,
                StrongExerciseCategory.WeightedBodyweight => (bodyweight + set.Weight.Value) * set.Reps.Value,
                StrongExerciseCategory.AssistedBodyweight => (bodyweight - set.Weight.Value) * set.Reps.Value,
                StrongExerciseCategory.RepsOnly => 0,
                StrongExerciseCategory.Cardio => 0,
                StrongExerciseCategory.Duration => 0,
                _ => 0
            };
        }

    }
}
