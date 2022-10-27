using Microsoft.AspNetCore.Components;
using Strongapp.Models;

namespace Strongapp.UI.Components
{
    public partial class WorkoutSummary
    {
        [Parameter]
        public StrongWorkoutSummary Workout { get; set; } = new StrongWorkoutSummary();

        [Parameter] 
        public EventCallback<StrongWorkoutSummary> OnRemoveWorkout { get; set; }

        public async Task RemoveWorkout()
        {
            await OnRemoveWorkout.InvokeAsync(Workout);
        }
    }
}
