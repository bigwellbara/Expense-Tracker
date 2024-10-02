using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Expense_Tracker.Models
{
    public class User
    {
        [BsonId]  // Map _id to UserId
        [BsonRepresentation(BsonType.String)]  
        public Guid UserId { get; set; } = Guid.NewGuid();

        [BsonElement("FirstName")]  
        public string FirstName { get; set; }

        [BsonElement("LastName")]  
        public string LastName { get; set; }

        [BsonElement("Email")] 
        [BsonRepresentation(BsonType.String)]
        public string Email { get; set; }

        [BsonElement("PasswordHash")]  
        public string PasswordHash { get; set; }

        [BsonElement("CreatedDate")]  
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [BsonElement("UpdatedDate")]  
        public DateTime UpdatedDate { get; set; } = DateTime.Now;
    }
}
