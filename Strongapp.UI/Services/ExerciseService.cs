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

        public async Task<StrongExercise> GetExerciseByName(string name)
        {
            var exercise = await http.GetFromJsonAsync<StrongExercise>($"exercises?name={name}");
            return exercise;
        }

        public async Task<List<StrongExerciseData>> GetExerciseHistory(string name)
        {
            var exercises = await http.GetFromJsonAsync<List<StrongExerciseData>>($"exercises/history?name={name}");
            return exercises;
        }

        public async Task<StrongExerciseData?> GetPrevious(string name)
        {
            var exercise = await http.GetFromJsonAsync<StrongExerciseData?>($"exercises/previous?name={name}");
            return exercise;
        }
        public async Task<List<StrongExercise>> GetExercises()
        {
            var exercises = await http.GetFromJsonAsync<List<StrongExercise>>($"exercises/all");
            return exercises;
        }
    }
}
