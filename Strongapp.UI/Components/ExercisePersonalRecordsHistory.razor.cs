using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Services;

namespace Strongapp.UI.Components
{
    public partial class ExercisePersonalRecordsHistory
    {
        [Parameter]
        public string ExerciseName { get; set; }

        [Parameter]
        public StrongExerciseCategory ExerciseCategory { get; set; }

        [Inject]
        public IExerciseService ExerciseService { get; set; } = default!;

        public StrongPersonalRecordsHistory PersonalRecordsHistory { get; set; }

        protected override async Task OnInitializedAsync()
        {
            PersonalRecordsHistory = await ExerciseService.GetPersonalRecordsHistory(ExerciseName);
        }
    }
}
