using Fluxor;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class FetchWorkoutsActionEffect : Effect<FetchWorkoutsAction>
    {
        private readonly IWorkoutService _workoutService;

        public FetchWorkoutsActionEffect(IWorkoutService workoutService)
        {
            _workoutService = workoutService;
        }

        public override async Task HandleAsync(FetchWorkoutsAction action, IDispatcher dispatcher)
        {
            var workouts = await _workoutService.GetWorkouts();
            dispatcher.Dispatch(new FetchWorkoutsResultAction(workouts));
        }
    }
}
