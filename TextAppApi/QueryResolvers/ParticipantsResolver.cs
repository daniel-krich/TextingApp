using HotChocolate;
using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;
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
    public class ParticipantsResolver
    {
        [UseOffsetPaging(DefaultPageSize = 15)]
        public async Task<IQueryable<UserEntity>> GetParticipants([Parent] ChatEntity chat, [Service] IDbContext dbContext)
        {
            return (await dbContext.FetchDBRefAsAsync<UserEntity>(chat.Participants)).AsQueryable();
        }

        public int GetParticipantsCount([Parent] ChatEntity chat)
        {
            return chat.Participants.Count();
        }
    }
}
