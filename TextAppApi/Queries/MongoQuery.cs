using HotChocolate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using MongoDB.Bson;
using MongoDB.Driver;
using HotChocolate.Types;
using HotChocolate.Data.MongoDb;
using HotChocolate.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TextAppApi.QueryTypes;

namespace TextAppApi.Queries
{
    public class MongoQuery
    {
        [UsePaging]
        [UseOffsetPaging]
        [UseProjection]
        [UseSorting]
        public IExecutable<UserEntity> CurrentUser([Service] IDbContext dbContext, [Service] IHttpContextAccessor httpContextAccessor)
        {
            return dbContext.GetUserCollection().AsQueryable().Where(o => o.Id == ObjectId.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber))).AsExecutable();
        }


        [UsePaging]
        [UseOffsetPaging]
        [UseProjection]
        [UseSorting]

        public IExecutable<ChatEntity> CurrentUserChats([Service] IDbContext dbContext, [Service] IHttpContextAccessor httpContextAccessor)
        {
            return dbContext.GetChatCollection().AsQueryable().Where(o => o.Participants.Contains(DbRefFactory.UserRef(ObjectId.Parse(httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber))))).AsExecutable();
        }
    }
}
