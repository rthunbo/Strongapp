using Microsoft.AspNetCore.Components;

namespace Strongapp.UI.Components
{
    public partial class RemovableSection
    {
        [Parameter]
        public string Title { get; set; }

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
