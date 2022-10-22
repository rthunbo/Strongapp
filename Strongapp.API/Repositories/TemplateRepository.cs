using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Strongapp.API.Database;
using Strongapp.Models;

namespace Strongapp.API.Repositories
{
    public class TemplateRepository
    {
        private readonly IMongoCollection<StrongTemplate> _templatesCollection;

        public TemplateRepository(
            IOptions<StrongDatabaseSettings> bookStoreDatabaseSettings)
        {
            var mongoClient = new MongoClient(
                bookStoreDatabaseSettings.Value.ConnectionString);

            var mongoDatabase = mongoClient.GetDatabase(
                bookStoreDatabaseSettings.Value.DatabaseName);

            _templatesCollection = mongoDatabase.GetCollection<StrongTemplate>(
                bookStoreDatabaseSettings.Value.TemplatesCollectionName);
        }

        public async Task<List<StrongTemplate>> GetAsync() =>
            await _templatesCollection.Find(_ => true).ToListAsync();

        public async Task<StrongTemplate?> GetAsync(string id) =>
            await _templatesCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(StrongTemplate newWorkout) =>
            await _templatesCollection.InsertOneAsync(newWorkout);

        public async Task UpdateAsync(string id, StrongTemplate updatedWorkout) =>
            await _templatesCollection.ReplaceOneAsync(x => x.Id == id, updatedWorkout);

        public async Task RemoveAsync(string id) =>
            await _templatesCollection.DeleteOneAsync(x => x.Id == id);
    }
}