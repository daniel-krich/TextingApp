using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TextAppApi.QueryTypes;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using MongoDB.Bson;
using MongoDB.Driver;
using HotChocolate.Data;

namespace TextAppApi.QueryResolvers
{
    public class MessagesResolver
    {
        public async Task<IExecutable<MessageEntity>> GetMessages([Parent] ChatEntity chat, [Service] IDbContext dbContext, IResolverContext context)
        {
            return (await dbContext.FetchDBRefAsAsync<MessageEntity>(chat.Messages)).AsQueryable().AsExecutable();
            //return dbContext.GetMessageCollection().AsQueryable().Where(o => chat.Messages.Contains(DbRefFactory.MessageRef(o.Id))).AsExecutable();
        }
    }
}
