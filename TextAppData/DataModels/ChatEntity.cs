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
    public class ChatEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public IList<MongoDBRef> Participants { get; set; }
        public IList<MongoDBRef> Messages { get; set; }
    }
}
