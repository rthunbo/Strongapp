using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongExerciseData
    {
        public string? ExerciseName { get; set; }

        public List<StrongExerciseSetData> Sets { get; set; } = new List<StrongExerciseSetData>();
    }
}
