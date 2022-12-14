using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongExerciseSetData
    {
        public int SetOrder { get; set; }

        public decimal? Weight { get; set; }

        public decimal? InitialWeight { get; set; }

        public int? Reps { get; set; }

        public int? InitialReps { get; set; }

        public decimal? Distance { get; set; }

        public decimal? InitialDistance { get; set; }

        public int? Seconds { get; set; }

        public int? InitialSeconds { get; set; }

        public bool IsComplete { get; set; }

        public List<StrongPersonalRecordType> PersonalRecords { get; set; } = new();
    }
}
