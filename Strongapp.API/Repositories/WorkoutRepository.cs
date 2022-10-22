using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Strongapp.API.Database;
using Strongapp.Models;

namespace Strongapp.API.Repositories
{
    public class WorkoutRepository
    {
        private readonly IMongoCollection<StrongWorkout> _workoutsCollection;

        public WorkoutRepository(
            IOptions<StrongDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _workoutsCollection = mongoDatabase.GetCollection<StrongWorkout>(
                bookStoreDatabaseSettings.Value.WorkoutsCollectionName);
        }

        public async Task<List<StrongWorkout>> GetAsync() =>
            await _workoutsCollection.Find(_ => true).ToListAsync();

        public async Task<StrongWorkout?> GetAsync(string id) =>
            await _workoutsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(StrongWorkout newWorkout) =>
            await _workoutsCollection.InsertOneAsync(newWorkout);

        public async Task UpdateAsync(string id, StrongWorkout updatedWorkout) =>
            await _workoutsCollection.ReplaceOneAsync(x => x.Id == id, updatedWorkout);

        public async Task RemoveAsync(string id) =>
            await _workoutsCollection.DeleteOneAsync(x => x.Id == id);

        public IQueryable<StrongWorkout> AsQueryable() =>
            _workoutsCollection.AsQueryable();
    }
}