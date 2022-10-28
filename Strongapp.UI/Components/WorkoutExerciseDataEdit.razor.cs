using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;
using System.Collections.Generic;
using System.Data;

namespace Strongapp.UI.Components
{
    public partial class WorkoutExerciseDataEdit
    {
        [Parameter]
        public StrongExerciseData ExerciseData { get; set; } = new StrongExerciseData();

        [Parameter] 
        public EventCallback<StrongExerciseData> OnRemoveExercise { get; set; }

        [Parameter]
        public EventCallback<string> OnModified { get; set; }

        [Parameter]
        public bool IsTemplate { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        public StrongExerciseData? Previous => State.Value.Exercises.FirstOrDefault(x => x.ExerciseName == ExerciseData.ExerciseName, new StrongExerciseWithMetadata()).PreviousPerformance;

        public bool IsMarkCompleteDisabled(StrongExerciseSetData Set)
        {
            if (ExerciseData.Category is StrongExerciseCategory.MachineOther or StrongExerciseCategory.Barbell
                or StrongExerciseCategory.Dumbbell or StrongExerciseCategory.WeightedBodyweight
                or StrongExerciseCategory.AssistedBodyweight)
            {
                return Set.Weight == null && Set.InitialWeight == null || Set.Reps == null && Set.InitialReps == null;
            }
            if (ExerciseData.Category is StrongExerciseCategory.RepsOnly)
            {
                return Set.Reps == null && Set.InitialReps == null;
            }
            if (ExerciseData.Category is StrongExerciseCategory.Cardio)
            {
                return Set.Seconds == null && Set.InitialSeconds == null || Set.Distance == null && Set.InitialDistance == null;
            }
            if (ExerciseData.Category is StrongExerciseCategory.Duration)
            {
                return Set.Seconds == null && Set.InitialSeconds == null;
            }
            return true;
        }

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
            if (value == null)
                Set.IsComplete = false;
            await OnModified.InvokeAsync();
        }

        private async Task RepsChanged(StrongExerciseSetData Set, int? value)
        {
            Set.Reps = value;
            if (value == null)
                Set.IsComplete = false;
            await OnModified.InvokeAsync();
        }

        private async Task SecondsChanged(StrongExerciseSetData Set, int? value)
        {
            Set.Seconds = value;
            if (value == null)
                Set.IsComplete = false;
            await OnModified.InvokeAsync();
        }

        private async Task DistanceChanged(StrongExerciseSetData Set, decimal
            ? value)
        {
            Set.Distance = value;
            if (value == null)
                Set.IsComplete = false;
            await OnModified.InvokeAsync();
        }

        public void MarkComplete(StrongExerciseSetData Set)
        {
            Set.Reps ??= Set.InitialReps;
            Set.Weight ??= Set.InitialWeight;
            Set.Seconds ??= Set.InitialSeconds;
            Set.Distance ??= Set.InitialDistance;
            Set.IsComplete = true;
        }

        public void MarkIncomplete(StrongExerciseSetData Set)
        {
            Set.IsComplete = false;
        }
    }
}
