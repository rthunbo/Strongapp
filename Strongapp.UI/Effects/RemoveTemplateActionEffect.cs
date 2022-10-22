using Fluxor;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class RemoveTemplateActionEffect : Effect<RemoveTemplateAction>
    {
        private readonly ITemplateService _templateService;

        public RemoveTemplateActionEffect(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public override async Task HandleAsync(RemoveTemplateAction action, IDispatcher dispatcher)
        {
            await _templateService.DeleteTemplate(action.Template.Id);
            dispatcher.Dispatch(new FetchTemplatesAction());
        }
    }
}
