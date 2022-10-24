using Fluxor;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class CreateTemplateActionEffect : Effect<CreateTemplateAction>
    {
        private readonly ITemplateService _templateService;

        public CreateTemplateActionEffect(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public override async Task HandleAsync(CreateTemplateAction action, IDispatcher dispatcher)
        {
            await _templateService.CreateTemplate(action.Template);
            dispatcher.Dispatch(new FetchTemplatesAction());
        }
    }
}
