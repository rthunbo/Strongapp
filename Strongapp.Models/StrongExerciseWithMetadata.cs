using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Strongapp.Models;

public class StrongExerciseWithMetadata
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    public string ExerciseName { get; set; } = null!;

    public StrongExerciseBodyPart BodyPart { get; set; }

    public StrongExerciseCategory Category { get; set; }

    public StrongExerciseData? PreviousPerformance { get; set; }

    public StrongExerciseSetData? BestSet { get; set; }
}