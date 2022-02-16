using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.DataEntities;

namespace TextAppApi.Core
{
    public class MessageEvent
    {
        public ChatEntity Chat { get; set; }
        public MessageEntity Message { get; set; }
    }
}
