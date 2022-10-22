using Blazored.Modal;
using Blazored.Modal.Services;
using Fluxor;
using Microsoft.AspNetCore.Components;
using Strongapp.Models;
using Strongapp.UI.Actions;
using Strongapp.UI.Modals;
using Strongapp.UI.Services;
using Strongapp.UI.Stores;

namespace Strongapp.UI.Components
{
    public partial class TemplatesAdministration
    {
        [CascadingParameter]
        public IModalService Modal { get; set; }

        [Inject]
        public IState<AppStore> State { get; set; } = default!;

        [Inject]
        public IDispatcher Dispatcher { get; set; } = default!;

        protected override void OnInitialized()
        {
            Dispatcher.Dispatch(new FetchTemplatesAction());
            Dispatcher.Dispatch(new FetchFoldersAction());
            base.OnInitialized();
        }

        public async Task CreateFolder()
        {
            var formModal = Modal.Show<EditFolderModal>("Create folder");
            var result = await formModal.Result;
            if (!result.Cancelled)
            {
                Dispatcher.Dispatch(new CreateFolderAction((StrongFolder) result.Data));
            }
        }

        public async Task RenameFolder(StrongFolder folder)
        {
            var modalParameters = new ModalParameters();
            modalParameters.Add("Folder", folder);
            var formModal = Modal.Show<EditFolderModal>("Rename folder", modalParameters);
            var result = await formModal.Result;
            if (!result.Cancelled)
            {
                Dispatcher.Dispatch(new RenameFolderAction((StrongFolder)result.Data));
            }
        }

        public void RemoveFolder(StrongFolder folder)
        {
            Dispatcher.Dispatch(new RemoveFolderAction(folder));
        }
    }
}
