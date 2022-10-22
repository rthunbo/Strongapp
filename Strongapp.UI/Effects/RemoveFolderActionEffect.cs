using Fluxor;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class RemoveFolderActionEffect : Effect<RemoveFolderAction>
    {
        private readonly IFolderService _folderService;

        public RemoveFolderActionEffect(IFolderService folderService)
        {
            _folderService = folderService;
        }

        public override async Task HandleAsync(RemoveFolderAction action, IDispatcher dispatcher)
        {
            await _folderService.DeleteFolder(action.Folder.Id);
            dispatcher.Dispatch(new FetchFoldersAction());
        }
    }
}
