using CsvHelper.Configuration;

namespace Strongapp.API.Database;

public sealed class CsvRecordMap : ClassMap<CsvRecord>
{
    public CsvRecordMap()
    {
        Map(m => m.Date).Index(0);
        Map(m => m.WorkoutName).Index(1);
        Map(m => m.Duration).Index(2);
        Map(m => m.ExerciseName).Index(3);
        Map(m => m.SetOrder).Index(4);
        Map(m => m.Weight).Index(5);
        Map(m => m.Reps).Index(6);
        Map(m => m.Distance).Index(7);
        Map(m => m.Seconds).Index(8);
        Map(m => m.Notes).Index(9);
        Map(m => m.WorkoutNotes).Index(10);
        Map(m => m.RPE).Index(11);
    }
}
