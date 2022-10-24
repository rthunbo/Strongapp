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
        private NavigationManager NavManager { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        [Inject]
        public IDispatcher Dispatcher { get; set; } = default!;

        public List<StrongExerciseWithMetadata> Exercises => State.Value.Exercises;

        protected async override Task OnInitializedAsync()
        {
            Template = await CreateTemplate();

            await base.OnInitializedAsync();
        }

        private async Task<StrongTemplate> CreateTemplate()
        {
            if (TemplateId == null)
            {
                return new StrongTemplate()
                {
                    FolderName = Folder
                };
            }
            else
                return await TemplateService.GetTemplateById(TemplateId);
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
                    Template.ExerciseData.Add(new StrongExerciseData(exercise.ExerciseName, exercise.BodyPart, exercise.Category, GetSets(exercise)));
                }

                IsModified = true;
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

            return sets;
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
