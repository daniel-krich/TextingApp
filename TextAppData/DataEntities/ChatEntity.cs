using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using TextAppData.DataContext;
using TextAppData.Enums;

namespace TextAppData.DataEntities
{
    public class ChatEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public string ChatId { get; set; }
        public DateTime LastActivity { get; set; }
        public ChatType Type { get; set; }
        public IList<MongoDBRef> Participants { get; set; }
        public IList<MongoDBRef> Messages { get; set; }
    }
}
