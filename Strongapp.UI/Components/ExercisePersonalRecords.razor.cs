using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Services;

namespace Strongapp.UI.Components
{
    public partial class ExercisePersonalRecords
    {
        [Parameter]
        public string ExerciseName { get; set; }

        [Parameter]
        public StrongExerciseCategory ExerciseCategory { get; set; }

        [Inject]
        public IExerciseService ExerciseService { get; set; } = default!;

        public StrongPersonalRecords PersonalRecords { get; set; }

        protected override async Task OnInitializedAsync()
        { 
            PersonalRecords = await ExerciseService.GetPersonalRecords(ExerciseName);
        }
    }
}
