using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IExerciseService
    {
        Task<StrongExercise> GetExerciseByName(string name);
        
        Task<List<StrongExerciseData>> GetExerciseHistory(string name);

        Task<StrongExerciseData?> GetPrevious(string name);

        Task<List<StrongExercise>> GetExercises();
    }
}