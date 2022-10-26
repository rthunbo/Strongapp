using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongPersonalRecords
    {
        public StrongPersonalRecord OneRM { get; set; }
        
        public StrongPersonalRecord Weight { get; set; }
        
        public StrongPersonalRecord MaxVolume { get; set; }

        public List<StrongPredictedPerformance> PredictedPerformances { get; set; }
    }
}
