using System.Net.Http.Json;
using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public class MeasurementService : IMeasurementService
    {
        private readonly HttpClient http;

        public MeasurementService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<List<StrongMeasurement>> GetMeasurements()
        {
            var measurements = await http.GetFromJsonAsync<List<StrongMeasurement>>("measurements");
            return measurements;
        }

        public async Task CreateMeasurement(StrongMeasurement measurement)
        {
            await http.PostAsJsonAsync<StrongMeasurement>($"measurements", measurement);
        }
    }
}
