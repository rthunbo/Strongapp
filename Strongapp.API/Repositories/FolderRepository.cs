using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Strongapp.API.Database;
using Strongapp.Models;

namespace Strongapp.API.Repositories
{
    public class FolderRepository
    {
        private readonly IMongoCollection<StrongFolder> _foldersCollection;

        public FolderRepository(
            IOptions<StrongDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _foldersCollection = mongoDatabase.GetCollection<StrongFolder>(
                bookStoreDatabaseSettings.Value.FoldersCollectionName);
        }

        public async Task<List<StrongFolder>> GetAsync() =>
            await _foldersCollection.Find(_ => true).ToListAsync();

        public async Task<StrongFolder?> GetAsync(string id) =>
            await _foldersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(StrongFolder newWorkout) =>
            await _foldersCollection.InsertOneAsync(newWorkout);

        public async Task UpdateAsync(string id, StrongFolder updatedWorkout) =>
            await _foldersCollection.ReplaceOneAsync(x => x.Id == id, updatedWorkout);

        public async Task RemoveAsync(string id) =>
            await _foldersCollection.DeleteOneAsync(x => x.Id == id);
    }
}