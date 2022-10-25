using Blazored.Modal;
using Blazored.Modal.Services;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Modals;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;
using System.Diagnostics.Metrics;
using System.Reflection;

namespace Strongapp.UI.Pages
{
    public partial class Exercises
    {
        public string SearchPhrase { get; set; }

        public StrongExerciseBodyPart? ExerciseBodyPart { get; set; }

        public StrongExerciseCategory? ExerciseCategory { get; set; }

        public List<StrongWorkout> Workouts { get; set; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        [Inject]
        public IWorkoutService WorkoutService { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        public List<StrongExerciseWithMetadata> SearchResults
        {
            get
            {
                var exercises = State.Value.Exercises;
                if (!string.IsNullOrEmpty(SearchPhrase))
                    exercises = exercises.Where(x => x.ExerciseName.Contains(SearchPhrase)).ToList();
                if (ExerciseBodyPart != null)
                    exercises = exercises.Where(x => x.BodyPart == ExerciseBodyPart).ToList();
                if (ExerciseCategory != null)
                    exercises = exercises.Where(x => x.Category == ExerciseCategory).ToList();
                return exercises;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            Workouts = await WorkoutService.GetWorkouts();
        }

        public void ShowExerciseHistoryModal(string exerciseName)
        {
            var workouts = Workouts.Where(x => x.ExerciseData.Any(x => x.ExerciseName == exerciseName)).OrderByDescending(x => x.Date).ToList();
            var modalParameters = new ModalParameters();
            modalParameters.Add("Workouts", workouts);
            modalParameters.Add("ExerciseName", exerciseName);
            Modal.Show<ExerciseHistoryModal>(exerciseName, modalParameters);
        }
    }
}
