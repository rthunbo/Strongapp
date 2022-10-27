﻿using System;
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

        public decimal? AddedVolume { get; set; }

        public decimal? Volume { get; set; }

        // ReSharper disable once InconsistentNaming
        public decimal? OneRM { get; set; }

        public bool HasWeightPr { get; set; }

        public bool HasRepsPr { get; set; }

        public bool HasDurationPr { get; set; }

        public bool HasDistancePr { get; set; }

        public bool HasVolumePr { get; set; }

        public bool HasOneRMPr { get; set; }


        public int GetNumberOfPrs()
        {
            int numPrs = 0;
            if (HasDurationPr) numPrs++;
            if (HasRepsPr) numPrs++;
            if (HasDistancePr) numPrs++;
            if (HasVolumePr) numPrs++;
            if (HasWeightPr) numPrs++;
            if (HasOneRMPr) numPrs++;
            return numPrs;
        }
    }
}
