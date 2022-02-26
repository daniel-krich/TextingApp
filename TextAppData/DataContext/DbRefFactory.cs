using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextAppData.DataContext
{
    public static class DbRefFactory
    {
        public static MongoDBRef UserRef(ObjectId objectId)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.UserCollection, objectId);
        }

        public static MongoDBRef MessageRef(ObjectId objectId)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.MessageCollection, objectId);
        }

        public static MongoDBRef UserRef(object p)
        {
            throw new NotImplementedException();
        }

        public static MongoDBRef ChatRef(ObjectId objectId)
        {
            return new MongoDBRef(DbContext.DbName, DbContext.ChatCollection, objectId);
        }
    }
}
