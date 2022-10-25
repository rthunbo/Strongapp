using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IExerciseService
    {
        Task<List<StrongExercise>> Search(string searchPhrase);

        Task<List<StrongExerciseDataHistory>> GetHistory(string name);

        Task<List<StrongExerciseWithMetadata>> GetExercises();
    }
}