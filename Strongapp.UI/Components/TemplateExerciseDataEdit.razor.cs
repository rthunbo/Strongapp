using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;
using System.Collections.Generic;
using System.Data;

namespace Strongapp.UI.Components
{
    public partial class TemplateExerciseDataEdit
    {
        [Parameter]
        public StrongExerciseData ExerciseData { get; set; } = new StrongExerciseData();

        [Parameter] 
        public EventCallback<StrongExerciseData> OnRemoveExercise { get; set; }

        [Parameter]
        public EventCallback<string> OnModified { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        public StrongHistoricExerciseData? Previous => State.Value.Exercises.First(x => x.ExerciseName == ExerciseData.ExerciseName).PreviousPerformance;

        public StrongExerciseCategory Category => State.Value.Exercises.First(x => x.ExerciseName == ExerciseData.ExerciseName).Category;

        protected async Task RemoveExercise()
        {
            await OnRemoveExercise.InvokeAsync(ExerciseData);
            await OnModified.InvokeAsync();
        }

        protected async Task AddSet()
        {
            ExerciseData.Sets.Add(new StrongExerciseSetData
            {
                SetOrder = ExerciseData.Sets.Count + 1
            });
            await OnModified.InvokeAsync();
        }

        protected async Task RemoveSet(StrongExerciseSetData set)
        {
            ExerciseData.Sets.Remove(set);
            for (int i = 0; i < ExerciseData.Sets.Count; i++)
                ExerciseData.Sets[i].SetOrder = i + 1;
            await OnModified.InvokeAsync();
        }

        private async Task WeightChanged(StrongExerciseSetData Set, decimal? value)
        {
            Set.Weight = value;
            await OnModified.InvokeAsync();
        }

        private async Task RepsChanged(StrongExerciseSetData Set, int? value)
        {
            Set.Reps = value;
            await OnModified.InvokeAsync();
        }

        private async Task SecondsChanged(StrongExerciseSetData Set, int? value)
        {
            Set.Seconds = value;
            await OnModified.InvokeAsync();
        }

        private async Task DistanceChanged(StrongExerciseSetData Set, decimal
            ? value)
        {
            Set.Distance = value;
            await OnModified.InvokeAsync();
        }
    }
}
