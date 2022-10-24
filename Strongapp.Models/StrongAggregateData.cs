using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongAggregateData
    {
        public List<StrongFolder> Folders { get; set; }

        public List<StrongTemplate> Templates { get; set; }
    }
}
