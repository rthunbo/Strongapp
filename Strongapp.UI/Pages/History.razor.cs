using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Strongapp.Models;
using Strongapp.UI.Services;
using System.Diagnostics;

namespace Strongapp.UI.Pages
{
    public partial class History
    {
        [Inject]
        public IWorkoutService WorkoutService { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; }

        private Virtualize<StrongWorkout> VirtualizeContainer { get; set; }

        private Stopwatch timer = new Stopwatch();

        protected async override Task OnInitializedAsync()
        {
            timer.Start();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            timer.Stop();
            Console.WriteLine($"Full rendering took {timer.ElapsedMilliseconds} ms.");
        }

        protected async Task RemoveWorkout(StrongWorkout workout)
        {
            await WorkoutService.DeleteWorkout(workout.Id);
            await VirtualizeContainer.RefreshDataAsync();
        }

        public void AddWorkout()
        {
            NavManager.NavigateTo("workouts/edit");
        }

        public async ValueTask<ItemsProviderResult<StrongWorkout>> LoadWorkouts(ItemsProviderRequest request)
        {
            Console.WriteLine($"Index: {request.StartIndex} Count: {request.Count}");
            StrongWorkoutList result = await WorkoutService.GetWorkouts(request.StartIndex, request.Count, request.CancellationToken);
            return new ItemsProviderResult<StrongWorkout>(result.Items, result.TotalItemCount);
        }
    }
}
