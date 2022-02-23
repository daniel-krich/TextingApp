using HotChocolate;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using MongoDB.Bson;
using MongoDB.Driver;
using HotChocolate.Data;

namespace TextAppApi.QueryResolvers
{
    public class ParticipantsResolver
    {
        public async Task<IExecutable<UserEntity>> GetParticipants([Parent] ChatEntity chat, [Service] IDbContext dbContext)
        {
            return (await dbContext.FetchDBRefAsAsync<UserEntity>(chat.Participants)).AsExecutable();
        }
    }
}
