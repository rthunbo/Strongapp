using Strongapp.Models;

namespace Strongapp.UI.Actions
{
    public record FetchAggregateDataResultAction(List<StrongFolder> Folders, List<StrongTemplate> Templates);
}
