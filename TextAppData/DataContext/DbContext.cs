using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using TextAppData.DataEntities;

namespace TextAppData.DataContext
{
    public class DbContext : IDbContext
    {
        private IMongoClient _connection;
        private IMongoDatabase _database;
        public static string Host { get => "127.0.0.1"; }
        public static int Port { get => 27017; }
        public static string DbName { get => "textapp_db"; }
        public static string UserCollection { get => "users_col"; }
        public static string ChatCollection { get => "chats_col"; }
        public static string MessageCollection { get => "messages_col"; }
        public static string SessionCollection { get => "session_col"; }

        public DbContext()
        {
            CreateConnection();
            CreateUniqueIndexesOnce();
        }

        private async void CreateUniqueIndexesOnce()
        {
            bool createUsersIndexes = !await (await GetUserCollection().Indexes.ListAsync()).AnyAsync();
            if(createUsersIndexes) // Create unique indexes only if they doesn't exist
            {
                var indexUsername = new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending(x => x.Username), new CreateIndexOptions<UserEntity>() { Collation = new Collation(locale: "en", strength: CollationStrength.Secondary), Unique=true });
                var indexEmail = new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending(x => x.Email), new CreateIndexOptions<UserEntity>() { Collation = new Collation(locale: "en", strength: CollationStrength.Secondary), Unique = true });
                var indexPassword = new CreateIndexModel<UserEntity>(Builders<UserEntity>.IndexKeys.Ascending(x => x.Password), new CreateIndexOptions<UserEntity>() { Collation = new Collation(locale: "en", strength: CollationStrength.Secondary) });
                //case insensitive + unique
                await GetUserCollection().Indexes.CreateOneAsync(indexUsername);
                await GetUserCollection().Indexes.CreateOneAsync(indexEmail);
                //case insensitive
                await GetUserCollection().Indexes.CreateOneAsync(indexPassword);
                //Trace.WriteLine("Created default indexes");
            }

            bool createSessionIndexes = !await (await GetSessionCollection().Indexes.ListAsync()).AnyAsync();
            if (createSessionIndexes) // Create unique indexes only if they doesn't exist
            {
                var indexUniqueToken = new CreateIndexModel<SessionEntity>(Builders<SessionEntity>.IndexKeys.Ascending(x => x.Token), new CreateIndexOptions<SessionEntity>() { Collation = new Collation(locale: "en", strength: CollationStrength.Secondary), Unique = true });
                var indexKeysExpire = Builders<SessionEntity>.IndexKeys.Ascending(o => o.ExpiresAt);
                var indexOptions = new CreateIndexOptions {  ExpireAfter = new TimeSpan(days: 14, 0,0,0) };
                var indexModel = new CreateIndexModel<SessionEntity>(indexKeysExpire, indexOptions);
                await GetSessionCollection().Indexes.CreateOneAsync(indexModel);
                await GetSessionCollection().Indexes.CreateOneAsync(indexUniqueToken);
                //Trace.WriteLine("Created default indexes");
            }
        }

        private void CreateConnection()
        {
            this._connection = new MongoClient(new MongoClientSettings
            {
                Server = new MongoServerAddress(Host, Port)
            });
            this._database = this._connection.GetDatabase(DbName);
        }

        public IMongoCollection<UserEntity> GetUserCollection()
        {
            return this._database.GetCollection<UserEntity>(UserCollection);
        }

        public IMongoCollection<ChatEntity> GetChatCollection()
        {
            return this._database.GetCollection<ChatEntity>(ChatCollection);
        }

        public IMongoCollection<MessageEntity> GetMessageCollection()
        {
            return this._database.GetCollection<MessageEntity>(MessageCollection);
        }

        public IMongoCollection<SessionEntity> GetSessionCollection()
        {
            return this._database.GetCollection<SessionEntity>(SessionCollection);
        }

        public async Task<TDocument> FetchDBRefAsAsync<TDocument>(MongoDBRef dbRef)
        {
            var collection = this._database.GetCollection<TDocument>(dbRef.CollectionName);

            var query = Builders<TDocument>.Filter.Eq("_id", dbRef.Id);
            return await (await collection.FindAsync(query)).FirstOrDefaultAsync();
        }

        public async Task<List<TDocument>> FetchDBRefAsAsync<TDocument>(IList<MongoDBRef> dbRefs)
        {
            List<TDocument> items = new List<TDocument>();
            foreach(MongoDBRef item in dbRefs)
            {
                var collection = this._database.GetCollection<TDocument>(item.CollectionName);
                var query = Builders<TDocument>.Filter.Eq("_id", item.Id);
                items.Add(await (await collection.FindAsync(query)).FirstOrDefaultAsync());
            }
            return items;
        }

        public async Task<UserEntity> TryGetUserEntityBySessionToken(string token)
        {
            var session = await GetSessionCollection().FindAsync(o => o.Token == token);
            if (await session.FirstOrDefaultAsync() is SessionEntity sess)
            {
                var res = await GetUserCollection().FindAsync(o => o.Id == sess.User.Id);
                if (await res.FirstOrDefaultAsync() is UserEntity model)
                    return model;
            }
            return null;
        }

        public async Task<UserEntity> TryGetUserEntityByCredentials(string username, string password)
        {
            var res = await GetUserCollection().FindAsync(o => o.Username == username && o.Password == password);
            if (await res.FirstOrDefaultAsync() is UserEntity model)
                return model;
            else
                return null;
        }

        public async Task<UserEntity> TryGetUserEntityByUsername(string username)
        {
            var res = await GetUserCollection().FindAsync(o => o.Username == username);
            if (await res.FirstOrDefaultAsync() is UserEntity model)
                return model;
            else
                return null;
        }
    }
}
