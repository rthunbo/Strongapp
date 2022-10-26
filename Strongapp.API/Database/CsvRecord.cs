namespace Strongapp.API.Database;

public class CsvRecord
{
    public DateTime Date { get; set; }

    public string WorkoutName { get; set; } = null!;

    public string Duration { get; set; } = null!;

    public string ExerciseName { get; set; } = null!;

    public int SetOrder { get; set; }

    public decimal Weight { get; set; }

    public int Reps { get; set; }

    public decimal Distance { get; set; }

    public int Seconds { get; set; }

    public string Notes { get; set; } = null!;

    public string WorkoutNotes { get; set; } = null!;

    public string RPE { get; set; } = null!;
}