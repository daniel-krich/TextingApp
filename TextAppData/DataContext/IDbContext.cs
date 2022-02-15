﻿using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextAppData.DataModels;

namespace TextAppData.DataContext
{
    public interface IDbContext
    {
        IMongoCollection<ChatEntity> GetChatCollection();
        IMongoCollection<MessageEntity> GetMessageCollection();
        IMongoCollection<UserEntity> GetUserCollection();
        IMongoCollection<SessionEntity> GetSessionCollection();
        Task<TDocument> FetchDBRefAsAsync<TDocument>(MongoDBRef dbRef);
        Task<List<TDocument>> FetchDBRefAsAsync<TDocument>(IList<MongoDBRef> dbRefs);
        Task<UserEntity> TryGetUserEntityBySessionToken(string token);
        Task<UserEntity> TryGetUserEntityByCredentials(string username, string password);
    }
}