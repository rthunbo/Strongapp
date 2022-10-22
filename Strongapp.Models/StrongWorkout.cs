using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongWorkout
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string? WorkoutName { get; set; }


        public List<StrongExerciseData> ExerciseData { get; set; } = new List<StrongExerciseData>();
    }
}
