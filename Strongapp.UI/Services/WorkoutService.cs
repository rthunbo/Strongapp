using System.Net.Http.Json;
using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public class WorkoutService : IWorkoutService
    {
        private readonly HttpClient http;

        public WorkoutService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<StrongWorkout> GetWorkoutById(string id)
        {
            var workout = await http.GetFromJsonAsync<StrongWorkout>($"workouts/{id}");
            return workout;
        }

        public async Task<string> CreateWorkout(StrongWorkout workout)
        {
            var result = await http.PostAsJsonAsync<StrongWorkout>($"workouts", workout);
            var createdWorkout = await result.Content.ReadFromJsonAsync<StrongWorkout>();
            return createdWorkout.Id;
        }

        public async Task UpdateWorkout(string id, StrongWorkout workout)
        {
            await http.PutAsJsonAsync<StrongWorkout>($"workouts/{id}", workout);
        }

        public async Task DeleteWorkout(string id)
        {
            await http.DeleteAsync($"workouts/{id}");
        }

        public async Task<StrongWorkoutList> GetWorkouts(int startIndex, int count, CancellationToken cancellationToken)
        {
            var workouts = await http.GetFromJsonAsync<StrongWorkoutList>($"workouts?start={startIndex}&count={count}", cancellationToken);
            return workouts;
        }
    }
}
