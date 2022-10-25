using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IWorkoutService
    {
        Task<StrongWorkout> GetWorkoutById(string id);

        Task<string> CreateWorkout(StrongWorkout workout);

        Task UpdateWorkout(string id, StrongWorkout workout);

        Task DeleteWorkout(string id);

        Task<StrongWorkoutList> GetWorkouts(int startIndex, int count, CancellationToken cancellationToken);
    }
}
