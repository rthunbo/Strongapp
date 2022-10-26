using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongPredictedPerformance
    {
        public int Reps { get; set; }

        public StrongBestPerformance? BestPerformance { get; set; }

        public int? Predicted { get; set; }
    }
}
