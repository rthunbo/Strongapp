using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IMeasurementService
    {
        Task<List<StrongMeasurement>> GetMeasurements();
    }
}
