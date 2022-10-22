using Fluxor;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Services;
using System.Net.WebSockets;

namespace Strongapp.UI.Effects
{
    public class CreateFolderActionEffect : Effect<CreateFolderAction>
    {
        private readonly IFolderService _folderService;

        public CreateFolderActionEffect(IFolderService folderService)
        {
            _folderService = folderService;
        }

        public override async Task HandleAsync(CreateFolderAction action, IDispatcher dispatcher)
        {
            await _folderService.CreateFolder(action.Folder);
            dispatcher.Dispatch(new FetchFoldersAction());
        }
    }
}
