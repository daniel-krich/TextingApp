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
    public class MessageSenderResolver
    {
        public async Task<UserEntity> GetSender([Parent] MessageEntity message, [Service] IDbContext dbContext)
        {
            return await dbContext.FetchDBRefAsAsync<UserEntity>(message.Sender);
            //return dbContext.GetUserCollection().AsQueryable().Where(o => o.Id == message.Sender.Id).AsExecutable();
        }
    }
}
