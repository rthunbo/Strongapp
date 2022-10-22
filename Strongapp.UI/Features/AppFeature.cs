using Fluxor;
using Strongapp.Models;
using Strongapp.UI.Stores;

namespace Strongapp.UI.Features
{
    public class AppFeature : Feature<AppStore>
    {
        public override string GetName() => nameof(AppStore);

        protected override AppStore GetInitialState()
            => new(
                IsTemplatesLoading: false,
                Templates: new List<StrongTemplate>(),
                IsFoldersLoading: false,
                Folders: new List<StrongFolder>(),
                IsTemplateLoading: false,
                CurrentTemplate: new StrongTemplate()
                );
    }
}
