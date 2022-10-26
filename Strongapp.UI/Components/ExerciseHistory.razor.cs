using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Services;

namespace Strongapp.UI.Components
{
    public partial class ExerciseHistory
    {
        [Parameter]
        public string ExerciseName { get; set; }

        [Parameter]
        public StrongExerciseCategory ExerciseCategory { get; set; }

        [Inject]
        public IExerciseService ExerciseService { get; set; } = default!;

        public async ValueTask<ItemsProviderResult<StrongExerciseDataHistory>> LoadExerciseHistory(ItemsProviderRequest request)
        {
            var result = await ExerciseService.GetHistory(ExerciseName, request.StartIndex, request.Count, request.CancellationToken);
            return new ItemsProviderResult<StrongExerciseDataHistory>(result.Items, result.TotalItemCount);
        }
    }
}
