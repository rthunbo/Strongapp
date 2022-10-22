using Microsoft.AspNetCore.Components;
using Strongapp.Models;

namespace Strongapp.UI.Components
{
    public partial class WorkoutSummary
    {
        [Parameter]
        public StrongWorkout Workout { get; set; } = new StrongWorkout();

        [Parameter] 
        public EventCallback<StrongWorkout> OnRemoveWorkout { get; set; }

        public async Task RemoveWorkout()
        {
            await OnRemoveWorkout.InvokeAsync(Workout);
        }
    }
}
