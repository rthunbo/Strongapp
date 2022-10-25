using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IExerciseService
    {
        Task<List<StrongExercise>> Search(string searchPhrase);

        Task<StrongExerciseDataHistoryList> GetHistory(string name, int start, int count, CancellationToken cancellationToken);

        Task<List<StrongExerciseWithMetadata>> GetExercises();
    }
}