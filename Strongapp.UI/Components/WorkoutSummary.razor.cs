using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Stores;

namespace Strongapp.UI.Components
{
    public partial class WorkoutSummary
    {
        [Parameter]
        public StrongWorkoutSummary Workout { get; set; } = new StrongWorkoutSummary();

        [Parameter] 
        public EventCallback<StrongWorkoutSummary> OnRemoveWorkout { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        public List<StrongExerciseWithMetadata> Exercises => State.Value.Exercises;

        public async Task RemoveWorkout()
        {
            await OnRemoveWorkout.InvokeAsync(Workout);
        }
    }
}
