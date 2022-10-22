using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongWorkoutList
    {
        public List<StrongWorkout> Items { get; set; }

        public int TotalItemCount { get; set; }
    }
}
