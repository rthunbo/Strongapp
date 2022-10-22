using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Strongapp.API.Database;
using Strongapp.Models;

namespace Strongapp.API.Repositories
{
    public class ExerciseRepository
    {
        private readonly IMongoCollection<StrongExercise> _exercisesCollection;

        public ExerciseRepository(
            IOptions<StrongDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _exercisesCollection = mongoDatabase.GetCollection<StrongExercise>(
                bookStoreDatabaseSettings.Value.ExercisesCollectionName);
        }

        public async Task<List<StrongExercise>> GetAsync() =>
            await _exercisesCollection.Find(_ => true).ToListAsync();

        public async Task<StrongExercise?> GetAsync(string id) =>
            await _exercisesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(StrongExercise newWorkout) =>
            await _exercisesCollection.InsertOneAsync(newWorkout);

        public async Task UpdateAsync(string id, StrongExercise updatedWorkout) =>
            await _exercisesCollection.ReplaceOneAsync(x => x.Id == id, updatedWorkout);

        public async Task RemoveAsync(string id) =>
            await _exercisesCollection.DeleteOneAsync(x => x.Id == id);
    }
}