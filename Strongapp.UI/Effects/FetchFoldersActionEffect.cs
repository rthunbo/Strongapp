using Fluxor;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class FetchFoldersActionEffect : Effect<FetchFoldersAction>
    {
        private readonly IFolderService _folderService;

        public FetchFoldersActionEffect(IFolderService folderService)
        {
            _folderService = folderService;
        }

        public override async Task HandleAsync(FetchFoldersAction action, IDispatcher dispatcher)
        {
            var folders = await _folderService.GetFolders();
            dispatcher.Dispatch(new FetchFoldersResultAction(folders));
        }
    }
}
