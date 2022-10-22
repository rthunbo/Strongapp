using Microsoft.AspNetCore.Components;

namespace Strongapp.UI.Components
{
    public partial class Grid<TItem>
    {
        [Parameter]
        public RenderFragment Header { get; set; } = default!;

        [Parameter]
        public RenderFragment<TItem> Row { get; set; } = default!;

        [Parameter]
        public IReadOnlyList<TItem> Items { get; set; } = default!;
    }
}
