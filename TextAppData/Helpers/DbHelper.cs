using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.DataContext;
using TextAppData.DataEntities;

namespace TextAppData.Helpers
{
    public static class DbHelper
    {
        public static async Task<string> CreateSessionId(this IMongoCollection<SessionEntity> session, UserEntity user)
        {
            SessionEntity newSession = new SessionEntity
            {
                ExpireAt = DateTime.Now,
                User = DbRefFactory.UserRef(user.Id)
            };
            await session.InsertOneAsync(newSession);
            return newSession.Id.ToString();
        }

        public static async Task<TDocument> FetchDBRefAsAsync<TDocument>(this IMongoCollection<TDocument> collection, MongoDBRef dbRef)
        {
            var query = Builders<TDocument>.Filter.Eq(nameof(MongoDBRef.Id), dbRef.Id);
            return await (await collection.FindAsync(query)).FirstOrDefaultAsync();
        }

        public static async Task<IEnumerable<TDocument>> FetchDBRefAsAsync<TDocument>(this IMongoCollection<TDocument> collection, IEnumerable<MongoDBRef> dbRefs)
        {
            if (dbRefs.Any())
            {
                var query = Builders<TDocument>.Filter.In(nameof(MongoDBRef.Id), dbRefs.Select(x => x.Id));
                return (await collection.FindAsync(query)).ToEnumerable();
            }
            else return new List<TDocument>();
        }

        public static async Task<UserEntity> TryGetUserEntityBySessionId(this IMongoCollection<SessionEntity> collection, IMongoCollection<UserEntity> userCollection, string sessionid)
        {
            try
            {
                var session = await collection.FindAsync(o => o.Id == ObjectId.Parse(sessionid));
                if (await session.FirstOrDefaultAsync() is SessionEntity sess)
                {
                    var res = await userCollection.FetchDBRefAsAsync(sess.User);
                    if (res is UserEntity user)
                        return user;
                }
                return default;
            }
            catch
            {
                return default;
            }
        }

        public static async Task<UserEntity> TryGetUserEntityByCredentials(this IMongoCollection<UserEntity> collection, string username, string password)
        {
            var res = await collection.FindAsync(o => o.Username == username && o.Password == password);
            if (await res.FirstOrDefaultAsync() is UserEntity model)
                return model;
            else
                return default;
        }

        public static async Task<UserEntity> TryGetUserEntityById(this IMongoCollection<UserEntity> collection, string id)
        {
            try
            {
                var res = await collection.FindAsync(o => o.Id == ObjectId.Parse(id));
                if (await res.FirstOrDefaultAsync() is UserEntity model)
                    return model;
                else
                    return default;
            }
            catch
            {
                return default;
            }
        }

        public static async Task<UserEntity> TryGetUserEntityByUsername(this IMongoCollection<UserEntity> collection, string username)
        {
            var res = await collection.FindAsync(o => o.Username == username);
            if (await res.FirstOrDefaultAsync() is UserEntity model)
                return model;
            else
                return default;
        }
    }
}
