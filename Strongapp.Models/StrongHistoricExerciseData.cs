using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongHistoricExerciseData
    {
        public DateTime Date { get; set; }

        public string? WorkoutName { get; set; }

        public List<StrongExerciseSetData> Sets { get; set; }
    }
}
