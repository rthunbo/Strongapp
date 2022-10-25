using static System.Net.WebRequestMethods;
using Strongapp.Models;
using System.Net.Http.Json;
using System.Xml.Linq;

namespace Strongapp.UI.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly HttpClient http;

        public ExerciseService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<List<StrongExerciseDataHistory>> GetHistory(string name)
        {
            var exercises = await http.GetFromJsonAsync<List<StrongExerciseDataHistory>>($"exercises/history?name={name}");
            return exercises;
        }

        public async Task<List<StrongExerciseWithMetadata>> GetExercises()
        {
            var exercises = await http.GetFromJsonAsync<List<StrongExerciseWithMetadata>>($"exercises");
            return exercises;
        }

        public async Task<List<StrongExercise>> Search(string searchPhrase)
        {
            var exercises = await http.GetFromJsonAsync<List<StrongExercise>>($"exercises/search?searchPhrase={searchPhrase}");
            return exercises;
        }
    }
}
