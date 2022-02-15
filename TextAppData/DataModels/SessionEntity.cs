using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.DataContext;

namespace TextAppData.DataModels
{
    public class SessionEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime ExpiresAt { get; set; }
        public string Token { get; set; }
        public MongoDBRef User { get; set; }
    }
}
