﻿using Microsoft.AspNetCore.Components;
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

        public List<StrongPersonalRecord> PersonalRecordsHistory { get; set; } = new List<StrongPersonalRecord>();

        protected override async Task OnInitializedAsync()
        {
            PersonalRecordsHistory = await ExerciseService.GetPersonalRecordsHistory(ExerciseName);
        }
    }
}
