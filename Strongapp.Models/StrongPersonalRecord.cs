using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongPersonalRecord
    {
        public DateTime? Date { get; set; }

        public StrongPersonalRecordType Type { get; set; }

        public decimal? Weight { get; set; }

        public int? Reps { get; set; }
    }
}
