using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongPersonalRecordsHistory
    {
        public List<StrongPersonalRecord> OneRM { get; set; }

        public List<StrongPersonalRecord> Weight { get; set; }

        public List<StrongPersonalRecord> MaxVolume { get; set; }
    }
}
