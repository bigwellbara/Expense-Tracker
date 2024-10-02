using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Expense_Tracker.Models
{
    public class Income
    {
        [BsonId]  // Map _id to IncomeId
        [BsonRepresentation(BsonType.String)]  
        public Guid IncomeId { get; set; } = Guid.NewGuid();

        [BsonElement("Amount")]  
        public decimal Amount { get; set; }

        [BsonElement("Source")]  
        public string Source { get; set; }

        [BsonElement("Description")]  
        public string Description { get; set; } = "";

        [BsonElement("IncomeDate")]  // Date when the income transaction occurred
        public DateTime IncomeDate { get; set; }

        [BsonElement("UserId")]  // Foreign key reference to User
        [BsonRepresentation(BsonType.String)]  // Store as a string (you can use ObjectId if preferred)
        public Guid UserId { get; set; }

        [BsonElement("CategoryId")]  // Foreign key reference to Category
        [BsonRepresentation(BsonType.String)]  // Store as a string (you can use ObjectId if preferred)
        public Guid CategoryId { get; set; }

        [BsonElement("CreatedDate")]  
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
