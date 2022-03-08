using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.DataEntities;
using TextAppData.DataContext;

namespace TextAppData.Factories
{
    public static class DbRefFactory
    {
        public static MongoDBRef UserRef(ObjectId objectId)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.UserCollection, objectId);
        }

        public static MongoDBRef UserRef(UserEntity entity)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.UserCollection, entity.Id);
        }

        public static MongoDBRef MessageRef(ObjectId objectId)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.MessageCollection, objectId);
        }
        
        public static MongoDBRef MessageRef(MessageEntity entity)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.MessageCollection, entity.Id);
        }

        public static MongoDBRef ChatRef(ObjectId objectId)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.ChatCollection, objectId);
        }

        public static MongoDBRef ChatRef(ChatEntity entity)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.ChatCollection, entity.Id);
        }
    }
}
