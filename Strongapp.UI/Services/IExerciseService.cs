using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IExerciseService
    {
        Task<List<StrongExerciseData>> GetHistory(string name);

        Task<List<StrongExerciseWithMetadata>> GetExercises();
    }
}