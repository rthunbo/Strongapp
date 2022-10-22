using Fluxor;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class CopyTemplateActionEffect : Effect<CopyTemplateAction>
    {
        private readonly ITemplateService _templateService;

        public CopyTemplateActionEffect(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public override async Task HandleAsync(CopyTemplateAction action, IDispatcher dispatcher)
        {
            var newTemplate = new StrongTemplate
            {
                TemplateName = action.Template.TemplateName + " Copy",
                FolderName = action.Template.FolderName,
                ExerciseData = action.Template.ExerciseData
            };
            await _templateService.CreateTemplate(newTemplate);

            dispatcher.Dispatch(new FetchTemplatesAction());
        }
    }
}
