using Fluxor;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class FetchExercisesActionEffect : Effect<FetchExercisesAction>
    {
        private readonly IExerciseService _exerciseService;

        public FetchExercisesActionEffect(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        public override async Task HandleAsync(FetchExercisesAction action, IDispatcher dispatcher)
        {
            var exercises = await _exerciseService.GetExercises();
            dispatcher.Dispatch(new FetchExercisesResultAction(exercises));
        }
    }
}
