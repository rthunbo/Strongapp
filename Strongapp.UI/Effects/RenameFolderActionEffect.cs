using Fluxor;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class RenameFolderActionEffect : Effect<RenameFolderAction>
    {
        private readonly IFolderService _folderService;

        public RenameFolderActionEffect(IFolderService folderService)
        {
            _folderService = folderService;
        }

        public override async Task HandleAsync(RenameFolderAction action, IDispatcher dispatcher)
        {
            await _folderService.UpdateFolder(action.Folder.Id, action.Folder);
            dispatcher.Dispatch(new FetchFoldersAction());
        }
    }
}
