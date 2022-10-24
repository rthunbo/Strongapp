using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Strongapp.API.Database;
using Strongapp.Models;

namespace Strongapp.API.Repositories
{
    public class MeasurementRepository
    {
        private readonly IMongoCollection<StrongMeasurement> _measurementsCollection;

        public MeasurementRepository(
            IOptions<StrongDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _measurementsCollection = mongoDatabase.GetCollection<StrongMeasurement>(
                bookStoreDatabaseSettings.Value.MeasurementsCollectionName);
        }

        public async Task<List<StrongMeasurement>> GetAsync() =>
            await _measurementsCollection.Find(_ => true).ToListAsync();

        public async Task<StrongMeasurement?> GetAsync(string id) =>
            await _measurementsCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(StrongMeasurement newWorkout) =>
            await _measurementsCollection.InsertOneAsync(newWorkout);

        public async Task UpdateAsync(string id, StrongMeasurement updatedWorkout) =>
            await _measurementsCollection.ReplaceOneAsync(x => x.Id == id, updatedWorkout);

        public async Task RemoveAsync(string id) =>
            await _measurementsCollection.DeleteOneAsync(x => x.Id == id);
    }
}