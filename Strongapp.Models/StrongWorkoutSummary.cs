using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongWorkoutSummary
    {
        public string? Id { get; set; }

        public DateTime Date { get; set; }

        public string? WorkoutName { get; set; }

        public decimal? Volume { get; set; }

        public int NumberOfPersonalRecords { get; set; }

        public List<StrongExerciseDataSummary> ExerciseData { get; set; }
    }
}
