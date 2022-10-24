using Blazored.Modal;
using Blazored.Modal.Services;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Modals;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;
using System.Resources;
using System.Security.Cryptography.X509Certificates;

namespace Strongapp.UI.Pages
{
    public partial class WorkoutEdit
    {
        [Parameter]
        public string WorkoutId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string Template { get; set; }

        public StrongWorkout? Workout { get; set; }

        [Inject]
        public IWorkoutService WorkoutService { get; set; }

        [Inject]
        public ITemplateService TemplateService { get; set; }

        [Inject]
        private NavigationManager NavManager { get; set; }

        [CascadingParameter]
        public IModalService Modal { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        [Inject]
        public IDispatcher Dispatcher { get; set; } = default!;

        public List<StrongExerciseWithMetadata> Exercises => State.Value.Exercises;

        protected async override Task OnInitializedAsync()
        {
            Workout = await CreateWorkout();

            await base.OnInitializedAsync();
        }

        private async Task<StrongWorkout> CreateWorkout()
        {
            if (WorkoutId == null)
            {
                if (Template == null)
                    return new StrongWorkout();
                else
                {
                    var template = await TemplateService.GetTemplateById(Template);
                    return CreateWorkoutFromTemplate(template);
                }
            }
            else
                return await WorkoutService.GetWorkoutById(WorkoutId);
        }

        protected async Task HandleValidSubmit()
        {
            var newWorkout = CreateWorkoutWithoutEmptySets();

            if (newWorkout.ExerciseData.Count == 0)
            {
                var formModal = Modal.Show<CancelWorkoutModal>("Cancel Workout?");
                var result = await formModal.Result;
                if (!result.Cancelled)
                {
                    NavManager.NavigateTo("/");
                }
            }
            else
            {
                var modalParams = new ModalParameters();
                modalParams.Add("HasEmptySets", WorkoutHasEmptySets());
                modalParams.Add("SubmitName", WorkoutId == null ? "Finish" : "Save");
                var formModal = Modal.Show<EndWorkoutModal>((WorkoutId == null ? "Finish" : "Save") + " Workout?", modalParams);
                var result = await formModal.Result;

                if (!result.Cancelled)
                {
                    if (WorkoutId == null)
                        await WorkoutService.CreateWorkout(newWorkout);
                    else
                        await WorkoutService.UpdateWorkout(WorkoutId, newWorkout);
                    NavManager.NavigateTo("/");
                }
            }
        }

        protected async Task AddExercises()
        {
            var formModal = Modal.Show<SelectExercisesModal>();
            var result = await formModal.Result;

            if (!result.Cancelled)
            {
                var selectedExercises = (List<StrongExerciseWithMetadata>)result.Data;
                foreach (var exercise in selectedExercises)
                {
                    Workout.ExerciseData.Add(new StrongExerciseData(exercise.ExerciseName, exercise.BodyPart, exercise.Category, GetSets(exercise)));
                }
            }
        }

        private List<StrongExerciseSetData> GetSets(StrongExerciseWithMetadata exercise)
        {
            var sets = new List<StrongExerciseSetData>
            {
                new StrongExerciseSetData { SetOrder = 1 }
            };
            var metadata = Exercises.First(x => x.ExerciseName == exercise.ExerciseName);
            if (metadata.PreviousPerformance is not null)
            {
                sets = metadata.PreviousPerformance.Sets;
            }

            return sets.Select(ConvertSet).ToList();
        }

        private StrongExerciseSetData ConvertSet(StrongExerciseSetData set)
        {
            return new StrongExerciseSetData
            {
                SetOrder = set.SetOrder,
                InitialDistance = set.Distance,
                InitialReps = set.Reps,
                InitialSeconds = set.Seconds,
                InitialWeight = set.Weight,
            };
        }

        public async Task CancelWorkout()
        {
            var newWorkout = CreateWorkoutWithoutEmptySets();

            if (newWorkout.ExerciseData.Count > 0)
            {
                var formModal = Modal.Show<CancelWorkoutModal>("Cancel Workout?");
                var result = await formModal.Result;
                if (!result.Cancelled)
                {
                    NavManager.NavigateTo("/");
                }
            }
            else
            {
                NavManager.NavigateTo("/");
            }
        }

        public async Task DeleteWorkout()
        {
            await WorkoutService.DeleteWorkout(WorkoutId);
            NavManager.NavigateTo("/");
        }

        protected void RemoveExercise(StrongExerciseData exerciseData)
        {
            Workout.ExerciseData.Remove(exerciseData);
        }

        private StrongWorkout CreateWorkoutFromTemplate(StrongTemplate template)
        {
            return new StrongWorkout
            {
                WorkoutName = template.TemplateName,
                ExerciseData = template.ExerciseData.Select(x => new StrongExerciseData
                {
                    ExerciseName = x.ExerciseName,
                    BodyPart = x.BodyPart,
                    Category = x.Category,
                    Sets = x.Sets.Select(ConvertSet).ToList()
                }).ToList(),
            };
        }

        private bool WorkoutHasEmptySets()
        {
            foreach (var exerciseData in Workout.ExerciseData)
            {
                foreach (var set in exerciseData.Sets)
                {
                    if (!IsValidSet(exerciseData.Category, set))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private StrongWorkout CreateWorkoutWithoutEmptySets()
        {
            var exerciseDataList = new List<StrongExerciseData>();
            foreach (var exerciseData in Workout.ExerciseData)
            {
                var newExerciseData = new StrongExerciseData
                {
                    BodyPart = exerciseData.BodyPart,
                    Category = exerciseData.Category,
                    Date = exerciseData.Date,
                    ExerciseName = exerciseData.ExerciseName,
                };
                var sets = new List<StrongExerciseSetData>();
                foreach (var set in exerciseData.Sets)
                {
                    if (IsValidSet(exerciseData.Category, set))
                    {
                        sets.Add(set);
                    }
                }
                if (sets.Count > 0)
                {
                    newExerciseData.Sets = sets;
                    exerciseDataList.Add(newExerciseData);
                }
            }
            var newWorkout = new StrongWorkout
            {
                Id = Workout.Id,
                Date = Workout.Date,
                WorkoutName = Workout.WorkoutName,
                ExerciseData = exerciseDataList,
            };
            return newWorkout;
        }

        protected bool IsValidSet(StrongExerciseCategory category, StrongExerciseSetData set)
        {
            if (category is StrongExerciseCategory.Barbell or StrongExerciseCategory.Dumbbell
                or StrongExerciseCategory.WeightedBodyweight or StrongExerciseCategory.AssistedBodyweight
                or StrongExerciseCategory.MachineOther)
            {
                if (set.Weight != null && set.Reps != null)
                {
                    return true;
                }
            }
            if (category is StrongExerciseCategory.RepsOnly)
            {
                if (set.Reps != null)
                {
                    return true;
                }
            }
            if (category is StrongExerciseCategory.Duration)
            {
                if (set.Seconds != null)
                {
                    return true;
                }
            }
            if (category is StrongExerciseCategory.Duration)
            {
                if (set.Seconds != null && set.Distance != null)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
