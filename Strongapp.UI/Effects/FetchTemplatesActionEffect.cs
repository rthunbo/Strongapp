using Fluxor;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class FetchTemplatesActionEffect : Effect<FetchTemplatesAction>
    {
        private readonly ITemplateService _templateService;

        public FetchTemplatesActionEffect(ITemplateService templateService)
        {
            _templateService = templateService;
        }

        public override async Task HandleAsync(FetchTemplatesAction action, IDispatcher dispatcher)
        {
            var templates = await _templateService.GetTemplates();
            dispatcher.Dispatch(new FetchTemplatesResultAction(templates));
        }
    }
}
