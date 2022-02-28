using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using TextAppData.DataEntities;

namespace TextAppData.DataContext
{
    public interface IDbContext
    {
        IMongoCollection<ChatEntity> GetChatCollection();
        IMongoCollection<MessageEntity> GetMessageCollection();
        IMongoCollection<UserEntity> GetUserCollection();
        IMongoCollection<SessionEntity> GetSessionCollection();
    }
}