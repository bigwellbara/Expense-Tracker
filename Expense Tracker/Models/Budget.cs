using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Expense_Tracker.Models
{
    public class Budget
    {
        [BsonId]  // Map _id to BudgetId
        [BsonRepresentation(BsonType.String)]  // Store the ID as a string (optional)
        public Guid BudgetId { get; set; } = Guid.NewGuid();

        [BsonElement("Amount")]  
        public decimal Amount { get; set; }

        [BsonElement("StartDate")]  // Start date of the budget period
        public DateTime StartDate { get; set; }

        [BsonElement("EndDate")]  // End date of the budget period
        public DateTime EndDate { get; set; }

        [BsonElement("UserId")]  // Foreign key reference to User
        [BsonRepresentation(BsonType.String)] 
        public Guid UserId { get; set; }

        [BsonElement("CategoryId")]  // Foreign key reference to Category
        [BsonRepresentation(BsonType.String)]  // Store as a string (you can use ObjectId if preferred)
        public Guid CategoryId { get; set; }

        [BsonElement("CreatedDate")]  // Automatically set the creation date
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
