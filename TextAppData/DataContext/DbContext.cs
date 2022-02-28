using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;

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
                //var indexUniqueToken = new CreateIndexModel<SessionEntity>(Builders<SessionEntity>.IndexKeys.Ascending(x => x.Token), new CreateIndexOptions<SessionEntity>() { Collation = new Collation(locale: "en", strength: CollationStrength.Secondary), Unique = true });
                var indexKeysExpire = Builders<SessionEntity>.IndexKeys.Ascending(o => o.ExpireAt);
                var indexOptions = new CreateIndexOptions {  ExpireAfter = new TimeSpan(days: 7, 0,0,0) };
                var indexModel = new CreateIndexModel<SessionEntity>(indexKeysExpire, indexOptions);
                await GetSessionCollection().Indexes.CreateOneAsync(indexModel);
                //await GetSessionCollection().Indexes.CreateOneAsync(indexUniqueToken);
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
    }
}
