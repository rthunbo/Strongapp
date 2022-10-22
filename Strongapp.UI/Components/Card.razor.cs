using Microsoft.AspNetCore.Components;
using Strongapp.Models;

namespace Strongapp.UI.Components
{
    public partial class Card
    {
        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public string SubTitle { get; set; }

        [Parameter]
        public string EditUrl { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;

        [Parameter]
        public EventCallback OnRemove { get; set; }

        public async Task Remove()
        {
            await OnRemove.InvokeAsync();
        }
    }
}
