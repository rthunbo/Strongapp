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

        [Parameter]
        public EventCallback OnViewRecordsHistory { get; set; }

        [Inject]
        public IExerciseService ExerciseService { get; set; } = default!;

        public List<StrongPersonalRecord> PersonalRecords { get; set; } = new List<StrongPersonalRecord>();

        public List<StrongPredictedPerformance> PredictedPerformances { get; set; } = new List<StrongPredictedPerformance>();

        protected override async Task OnInitializedAsync()
        { 
            PersonalRecords = await ExerciseService.GetPersonalRecords(ExerciseName);
            PredictedPerformances = await ExerciseService.GetPredictedPerformances(ExerciseName);
        }

        public async Task ViewRecordsHistory()
        {
            await OnViewRecordsHistory.InvokeAsync();
        }
    }
}
