using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using TextAppData.DataContext;

namespace TextAppData.DataModels
{
    public class MessageEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public MongoDBRef Sender { get; set; }
        public DateTime Time { get; set; }
        public string Message { get; set; }
    }
}
