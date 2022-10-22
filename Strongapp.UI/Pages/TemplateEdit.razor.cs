using Blazored.Modal.Services;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Components;
using Strongapp.UI.Modals;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;
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

        public StrongTemplate Template { get; set; } = new StrongTemplate();

        [Inject]
        public ITemplateService TemplateService { get; set; }

        [Inject]
        public IExerciseService ExerciseService { get; set; }

        [Inject]
        private NavigationManager NavManager { get; set; }
        
        protected async override Task OnInitializedAsync()
        {
            if (TemplateId == null)
            {
                Template = new StrongTemplate()
                {
                    FolderName = Folder
                };
            }
            else
                Template = await TemplateService.GetTemplateById(TemplateId);

            await base.OnInitializedAsync();
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
                var selectedExercises = (List<StrongExercise>) result.Data;
                foreach (var exercise in selectedExercises)
                {
                    var sets = new List<StrongExerciseSetData>
                    {
                        new StrongExerciseSetData { SetOrder = 1 }
                    };
                    var exerciseHistory = await ExerciseService.GetExerciseHistory(exercise.ExerciseName);
                    if (exerciseHistory.Any())
                    {
                        sets = exerciseHistory.Last().Sets;
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
                await TemplateService.DeleteTemplate(TemplateId);
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
