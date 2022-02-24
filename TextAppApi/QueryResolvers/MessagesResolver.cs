using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using TextAppData.Enums;

namespace TextAppApi.QueryResolvers
{
    public class MessagesResolver
    {
        [UseOffsetPaging(DefaultPageSize = 15)]
        public async Task<IQueryable<MessageEntity>> GetMessages([Parent] ChatEntity chat, [Service] IDbContext dbContext)
        {
            return (await dbContext.FetchDBRefAsAsync<MessageEntity>(chat.Messages)).AsQueryable();
        }

        public async Task<MessageEntity> GetLastMessage([Parent] ChatEntity chat, [Service] IDbContext dbContext)
        {
            return await dbContext.FetchDBRefAsAsync<MessageEntity>(chat.Messages.LastOrDefault());
        }

        public int GetMessagesCount([Parent] ChatEntity chat)
        {
            return chat.Messages.Count();
        }
    }
}
