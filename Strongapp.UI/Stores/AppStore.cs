using Strongapp.Models;

namespace Strongapp.UI.Stores
{
    public record AppStore(
        bool IsTemplatesLoading,
        List<StrongTemplate> Templates,
        bool IsFoldersLoading,
        List<StrongFolder> Folders,
        bool IsTemplateLoading,
        StrongTemplate CurrentTemplate
        );
}