using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;

namespace Strongapp.UI.Components
{
    public partial class TemplateSummary
    {
        [Inject]
        public IDispatcher Dispatcher { get; set; }

        [Parameter]
        public StrongTemplate Template { get; set; }

        public async Task CopyTemplate()
        {
            Dispatcher.Dispatch(new CopyTemplateAction(Template));
        }

        public void RemoveTemplate()
        {
            Dispatcher.Dispatch(new RemoveTemplateAction(Template));
        }
    }
}
