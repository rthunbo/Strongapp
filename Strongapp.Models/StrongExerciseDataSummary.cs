using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongExerciseDataSummary
    {
        public string ExerciseName { get; set; }

        public int NumberOfSets { get; set; }

        public StrongExerciseSetData? BestSet { get; set; }
    }
}
