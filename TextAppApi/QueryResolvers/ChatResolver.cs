using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using TextAppData.Enums;

namespace TextAppApi.QueryResolvers
{
    public class ChatResolver
    {
        [UseOffsetPaging(DefaultPageSize = 15)]
        public async Task<IQueryable<UserEntity>> GetParticipants([Parent] ChatEntity chat, [Service] IDbContext dbContext)
        {
            return (await dbContext.FetchDBRefAsAsync<UserEntity>(chat.Participants)).AsQueryable();
        }

        [UseOffsetPaging(DefaultPageSize = 15)]
        public async Task<IQueryable<MessageEntity>> GetMessages([Parent] ChatEntity chat, [Service] IDbContext dbContext)
        {
            return (await dbContext.FetchDBRefAsAsync<MessageEntity>(chat.Messages)).AsQueryable();
        }

        public async Task<UserEntity> GetSender([Parent] MessageEntity message, [Service] IDbContext dbContext)
        {
            return await dbContext.FetchDBRefAsAsync<UserEntity>(message.Sender);
        }

        public async Task<string> GetChatId([Parent] ChatEntity chat, [Service] IDbContext dbContext, [Service] IHttpContextAccessor httpContextAccessor)
        {
            switch(chat.Type)
            {
                case ChatType.Regular:
                    return (await dbContext.FetchDBRefAsAsync<UserEntity>(chat.Participants.Where(o => o.Id != ObjectId.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber))).FirstOrDefault())).Username;
                case ChatType.Group:
                    return chat.ChatId;
                default:
                    return default;
            }
        }

        public async Task<MessageEntity> GetLastMessage([Parent] ChatEntity chat, [Service] IDbContext dbContext)
        {
            return await dbContext.FetchDBRefAsAsync<MessageEntity>(chat.Messages.LastOrDefault());
        }

        public int GetMessagesCount([Parent] ChatEntity chat)
        {
            return chat.Messages.Count();
        }

        public int GetParticipantsCount([Parent] ChatEntity chat)
        {
            return chat.Participants.Count();
        }
    }
}
