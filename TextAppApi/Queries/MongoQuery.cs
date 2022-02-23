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
using TextAppData.Enums;
//using TextAppApi.QueryTypes;

namespace TextAppApi.Queries
{
    public class MongoQuery
    {
        private IDbContext _dbContext;
        private IHttpContextAccessor _httpContextAccessor;
        public MongoQuery([Service] IDbContext dbContext, [Service] IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        [UsePaging, UseOffsetPaging, UseProjection, UseSorting]
        public IQueryable<UserEntity> GetCurrentUser()
        {
            return _dbContext.GetUserCollection().AsQueryable().Where(o => o.Id == ObjectId.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber)));
        }


        [UsePaging, UseOffsetPaging, UseProjection, UseSorting]
        public IQueryable<ChatEntity> GetCurrentUserChats()
        {
            return _dbContext.GetChatCollection().AsQueryable().Where(o => o.Participants.Contains(DbRefFactory.UserRef(ObjectId.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber)))));
        }

        [UsePaging, UseOffsetPaging, UseProjection, UseSorting]
        public async Task<IQueryable<ChatEntity>> GetUserChatByChatId(string chatId, ChatType chatType)
        {
            
            UserEntity user = await _dbContext.TryGetUserEntityByUsername(chatId);
            IQueryable<ChatEntity> query = GetCurrentUserChats().Where(o => chatType == o.Type);
            switch (chatType)
            {
                case ChatType.Regular:
                    if (user is UserEntity)
                    {
                        return query.Where(o => o.Participants.Contains(DbRefFactory.UserRef(user.Id)));
                    }
                    else return Enumerable.Empty<ChatEntity>().AsQueryable();

                case ChatType.Group:
                    try
                    {
                        var tryConvert = Convert.ToUInt64(chatId);
                        return query.Where(o => o.GroupId == tryConvert);
                    }
                    catch (FormatException) { return Enumerable.Empty<ChatEntity>().AsQueryable(); }
                    catch (OverflowException) { return Enumerable.Empty<ChatEntity>().AsQueryable(); }

                default:
                return Enumerable.Empty<ChatEntity>().AsQueryable();
             }
        }
    }
}
