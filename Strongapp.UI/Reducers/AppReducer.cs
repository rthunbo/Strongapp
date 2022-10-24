using Fluxor;
using Strongapp.UI.Actions;
using Strongapp.UI.Stores;

namespace Strongapp.UI.Reducers
{
    public static class AppReducer
    {
        [ReducerMethod]
        public static AppStore ReduceFetchTemplatesAction(AppStore state, FetchTemplatesAction action)
            => state with { IsTemplatesLoading = true };

        [ReducerMethod]
        public static AppStore ReduceFetchTemplatesResultAction(AppStore state, FetchTemplatesResultAction action)
            => state with { IsTemplatesLoading = false, Templates = action.Templates };

        [ReducerMethod]
        public static AppStore ReduceFetchFoldersAction(AppStore state, FetchFoldersAction action)
            => state with { IsFoldersLoading = true };

        [ReducerMethod]
        public static AppStore ReduceFetchFoldersResultAction(AppStore state, FetchFoldersResultAction action)
            => state with { IsFoldersLoading = false, Folders = action.Folders };

        [ReducerMethod]
        public static AppStore ReduceFetchExercisesResultAction(AppStore state, FetchExercisesResultAction action)
            => state with { Exercises = action.Exercises };

        [ReducerMethod]
        public static AppStore ReduceFetchAggregateDataAction(AppStore state, FetchAggregateDataAction action)
            => state with { IsLoading = true };

        [ReducerMethod]
        public static AppStore ReduceFetchAggregateDataResultAction(AppStore state, FetchAggregateDataResultAction action)
            => state with { IsLoading = false, Folders = action.Folders, Templates = action.Templates };
    }
}
