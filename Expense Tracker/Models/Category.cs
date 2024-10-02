using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expense_Tracker.Models
{
    public class Category
    {
        

        [BsonId]  // Map _id to CategoryId
        [BsonRepresentation(BsonType.String)]  // Store the ID as a string (optional)
        public Guid CategoryId { get; set; } = Guid.NewGuid();

        [BsonElement("Title")]  // Optional: map the property to the "Title" field in MongoDB
        public string Title { get; set; }

        [BsonElement("Description")]
        public string Description { get; set; } = "";


        [BsonElement("Type")]
        public string Type { get; set; } = "Expense";

        [BsonElement("CreatedDate")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
