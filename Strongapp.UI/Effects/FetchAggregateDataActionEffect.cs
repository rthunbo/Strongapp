using Fluxor;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class FetchAggregateDataActionEffect : Effect<FetchAggregateDataAction>
    {
        private readonly IAggregateDataService _aggregateDataService;

        public FetchAggregateDataActionEffect(IAggregateDataService aggregateDataService)
        {
            _aggregateDataService = aggregateDataService;
        }

        public override async Task HandleAsync(FetchAggregateDataAction action, IDispatcher dispatcher)
        {
            var aggregateData = await _aggregateDataService.GetAggregateData();
            dispatcher.Dispatch(new FetchAggregateDataResultAction(aggregateData.Folders, aggregateData.Templates));
        }
    }
}
