using Blazored.Modal.Services;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Components;
using Strongapp.UI.Modals;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;
using System;
using System.Reflection;

namespace Strongapp.UI.Pages
{
    public partial class TemplateEdit
    {
        [CascadingParameter]
        public IModalService Modal { get; set; }

        [Parameter]
        public string TemplateId { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string Folder { get; set; }

        public bool IsModified { get; set; }

        public StrongTemplate? Template { get; set; }

        [Inject]
        public ITemplateService TemplateService { get; set; }

        [Inject]
        public IExerciseService ExerciseService { get; set; }

        [Inject]
        private NavigationManager NavManager { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        [Inject]
        public IDispatcher Dispatcher { get; set; } = default!;

        public List<StrongExerciseWithMetadata> Exercises => State.Value.Exercises;

        protected async override Task OnInitializedAsync()
        {
            Template = CreateTemplate();
            State.StateChanged += State_StateChanged;

            await base.OnInitializedAsync();
        }

        private StrongTemplate? CreateTemplate()
        {
            if (State.Value.Templates.Count > 0)
            {
                if (TemplateId == null)
                {
                    return new StrongTemplate()
                    {
                        FolderName = Folder
                    };
                }
                else
                    return State.Value.Templates.FirstOrDefault(x => x.Id == TemplateId);
            }
            return null;
        }

        private void State_StateChanged(object? sender, EventArgs e)
        {
           Template ??= CreateTemplate();
        }

        protected override void Dispose(bool disposing)
        {
            State.StateChanged -= State_StateChanged;
        }

        public void SetModified()
        {
            IsModified = true;
        }

        public void NameChanged(string? value)
        {
            Template.TemplateName = value;
            IsModified = true;
        }

        protected async Task HandleValidSubmit()
        {
            if (TemplateId == null)
                await TemplateService.CreateTemplate(Template);
            else
                await TemplateService.UpdateTemplate(TemplateId, Template);
            NavManager.NavigateTo("/");
        }

        protected void RemoveExercise(StrongExerciseData exerciseData)
        {
            Template.ExerciseData.Remove(exerciseData);
        }

        protected async Task AddExercises()
        {
            var formModal = Modal.Show<SelectExercisesModal>();
            var result = await formModal.Result;

            if (!result.Cancelled)
            {
                var selectedExercises = (List<StrongExerciseWithMetadata>) result.Data;
                foreach (var exercise in selectedExercises)
                {
                    var sets = new List<StrongExerciseSetData>
                    {
                        new StrongExerciseSetData { SetOrder = 1 }
                    };
                    if (Exercises.First(x => x.ExerciseName == exercise.ExerciseName).PreviousPerformance is  null)
                    {
                        sets = Exercises.First(x => x.ExerciseName == exercise.ExerciseName).PreviousPerformance.Sets;
                    }
                    Template.ExerciseData.Add(new StrongExerciseData(exercise.ExerciseName, exercise.BodyPart, exercise.Category, sets));
                }

                IsModified = true;
            }
        }

        protected async Task DeleteTemplate()
        {
            var formModal = Modal.Show<DeleteTemplateModal>("Delete Template?");
            var result = await formModal.Result;
            if (!result.Cancelled)
            {
                Dispatcher.Dispatch(new RemoveTemplateAction(Template));
                NavManager.NavigateTo("/");
            }
        }

        protected async Task CancelTemplate()
        {
            if (TemplateId == null)
            {
                var formModal = Modal.Show<DiscardTemplateModal>("Discard Template?");
                var result = await formModal.Result;
                if (!result.Cancelled)
                {
                    NavManager.NavigateTo("/");
                }
            }
            else if (IsModified)
            {
                var formModal = Modal.Show<RevertChangesModal>("Revert Changes?");
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
    }
}
