﻿using static System.Net.WebRequestMethods;
using Strongapp.Models;
using System.Net.Http.Json;
using System.Xml.Linq;
using System.Threading;

namespace Strongapp.UI.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly HttpClient http;

        public ExerciseService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<StrongExerciseDataHistoryList> GetHistory(string name, int start, int count, CancellationToken cancellationToken)
        {
            var exercises = await http.GetFromJsonAsync<StrongExerciseDataHistoryList>($"exercises/history?name={name}&start={start}&count={count}", cancellationToken);
            return exercises;
        }

        public async Task<List<StrongExerciseWithMetadata>> GetExercises()
        {
            var exercises = await http.GetFromJsonAsync<List<StrongExerciseWithMetadata>>($"exercises");
            return exercises;
        }

        public async Task<StrongPersonalRecords> GetPersonalRecords(string name)
        {
            var personalRecords = await http.GetFromJsonAsync<StrongPersonalRecords>($"exercises/records?name={name}");
            return personalRecords;
        }
    }
}
