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
        public async Task<IQueryable<MessageEntity>> GetMessages([Parent] ChatEntity chat, [Service] IDbContext dbContext, IResolverContext context)
        {
            return (await dbContext.FetchDBRefAsAsync<MessageEntity>(chat.Messages)).FilterByContext(context).AsQueryable();
        }
    }
}
