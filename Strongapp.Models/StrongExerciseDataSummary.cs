using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public record StrongExerciseDataSummary(string? ExerciseName, int NumberOfSets, StrongExerciseSetData? BestSet);
}
