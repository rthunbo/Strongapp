﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongExerciseData
    {
        public StrongExerciseData()
        {

        }

        public StrongExerciseData(string exerciseName, StrongExerciseBodyPart bodyPart, StrongExerciseCategory category, List<StrongExerciseSetData> sets)
        {
            ExerciseName = exerciseName;
            BodyPart = bodyPart;
            Category = category;
            Sets = sets;
        }

        public DateTime Date { get; set; } = DateTime.Now;

        public string? ExerciseName { get; set; }

        public StrongExerciseBodyPart BodyPart { get; set; }

        public StrongExerciseCategory Category { get; set; }

        public List<StrongExerciseSetData> Sets { get; set; } = new List<StrongExerciseSetData>();

        public decimal? GetVolume()
        {
            return Sets.Sum(x => x.Volume);
        }

        public int GetNumberOfPrs()
        {
            return Sets.Sum(x => x.GetNumberOfPrs());
        }
    }
}
