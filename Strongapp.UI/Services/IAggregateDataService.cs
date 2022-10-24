using Strongapp.Models;

namespace Strongapp.UI.Services
{
    public interface IAggregateDataService
    {
        Task<StrongAggregateData> GetAggregateData();
    }
}
