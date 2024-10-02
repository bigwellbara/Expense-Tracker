using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Expense_Tracker.Models
{
    public class Expense
    {
        [BsonId]  // Map _id to ExpenseId
        [BsonRepresentation(BsonType.String)]  
        public Guid ExpenseId { get; set; } = Guid.NewGuid();

        [BsonElement("Amount")]  
        public decimal Amount { get; set; }

        [BsonElement("Title")]  
        public string Title { get; set; }

        [BsonElement("Description")] 
        public string Description { get; set; } = "";

        [BsonElement("ExpenseDate")]  
        public DateTime ExpenseDate { get; set; }

        [BsonElement("UserId")]  
        [BsonRepresentation(BsonType.String)]  
        public Guid UserId { get; set; }

        [BsonElement("CategoryId")]  
        [BsonRepresentation(BsonType.String)]  
        public Guid CategoryId { get; set; }

        [BsonElement("CreatedDate")] 
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
