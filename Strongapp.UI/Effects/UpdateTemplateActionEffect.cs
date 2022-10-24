using Fluxor;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class UpdateTemplateActionEffect : Effect<UpdateTemplateAction>
    {
        private readonly ITemplateService _templateService;

        public UpdateTemplateActionEffect(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public override async Task HandleAsync(UpdateTemplateAction action, IDispatcher dispatcher)
        {
            await _templateService.UpdateTemplate(action.Template.Id, action.Template);
            dispatcher.Dispatch(new FetchTemplatesAction());
        }
    }
}
