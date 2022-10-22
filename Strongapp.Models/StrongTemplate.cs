using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strongapp.Models
{
    public class StrongTemplate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string? TemplateName { get; set; }

        public string? FolderName { get; set; }

        public bool IsExampleTemplate { get; set; }

        public List<StrongExerciseData> ExerciseData { get; set; } = new List<StrongExerciseData>();
    }
}
