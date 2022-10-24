using System.Net.Http.Json;
using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public class AggregateDataService : IAggregateDataService
    {
        private readonly HttpClient http;

        public AggregateDataService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<StrongAggregateData> GetAggregateData()
        {
            var aggregateData = await http.GetFromJsonAsync<StrongAggregateData>("aggregate");
            return aggregateData;
        }
    }
}
