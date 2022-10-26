using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IExerciseService
    {
        Task<StrongExerciseDataHistoryList> GetHistory(string name, int start, int count, CancellationToken cancellationToken);

        Task<List<StrongExerciseWithMetadata>> GetExercises();

        Task<StrongPersonalRecords> GetPersonalRecords(string name);
    }
}