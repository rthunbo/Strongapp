using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.UI.Actions;

namespace Strongapp.UI
{
    public partial class App
    {
        [Inject]
        public IDispatcher Dispatcher { get; set; } = default!;

        protected override void OnInitialized()
        {
            Dispatcher.Dispatch(new FetchAggregateDataAction());
            Dispatcher.Dispatch(new FetchExercisesAction());
            Dispatcher.Dispatch(new FetchWorkoutsAction());
            base.OnInitialized();
        }
    }
}
