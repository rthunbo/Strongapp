using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class Helpers
    {
        public static StrongExerciseSetData? GetBestSet(StrongExerciseCategory exerciseCategory, IEnumerable<StrongExerciseSetData> sets)
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
    }
}
