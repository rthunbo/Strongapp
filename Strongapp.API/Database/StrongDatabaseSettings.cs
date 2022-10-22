namespace Strongapp.API.Database
{
    public class StrongDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string WorkoutsCollectionName { get; set; } = null!;

        public string ExercisesCollectionName { get; set; } = null!;

        public string TemplatesCollectionName { get; set; } = null!;

        public string FoldersCollectionName { get; set; } = null!;
    }
}
