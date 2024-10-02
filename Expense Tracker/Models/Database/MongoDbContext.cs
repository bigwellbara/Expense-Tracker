using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Expense_Tracker.Models.Database
{
    public class MongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IOptions<MongoDbSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.DatabaseName);
        }

        public IMongoCollection<Transaction> Transactions => _database.GetCollection<Transaction>("Transactions");
        public IMongoCollection<Category> Categories => _database.GetCollection<Category>("Categories");
    }
}
