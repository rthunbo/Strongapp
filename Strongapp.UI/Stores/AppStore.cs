using Strongapp.Models;

namespace Strongapp.UI.Stores
{
    public record AppStore(
        bool IsTemplatesLoading,
        List<StrongTemplate> Templates,
        bool IsFoldersLoading,
        List<StrongFolder> Folders,
        List<StrongExerciseWithMetadata> Exercises,
        List<StrongWorkout> Workouts,
        bool IsLoading
        );
}