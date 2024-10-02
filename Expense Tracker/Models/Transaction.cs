using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Transaction
    {
        [BsonId]  // Map _id to CategoryId
        [BsonRepresentation(BsonType.String)]  // Store the ID as a string (optional)
        public Guid TransactionId { get; set; } = Guid.NewGuid();

        public Guid CategoryId { get; set; }
        public Category Category { get; set; }

        public decimal Amount { get; set; }

        [BsonElement("Note")]
        public string? Note { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;
    }
}
