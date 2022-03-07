using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppApi.Core;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using TextAppData.Enums;
using TextAppData.Helpers;

namespace TextAppApi.Queries
{
    public partial class DbQueries
    {
        [Authorize]
        [UseOffsetPaging(DefaultPageSize = 15)]
        public async Task<IQueryable<ChatEntity>> GetUserChats()
        {
            var sessionId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
            var User = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), sessionId);
            if (User is UserEntity)
            {
                return _dbContext.GetChatCollection()
                    .AsQueryable()
                    .Where(o => o.Participants.Contains(DbRefFactory.UserRef(User.Id))).OrderByDescending(o => o.LastActivity);
            }
            else
            {
                throw new ApplicationException("Session Expired");
            }
        }

        [Authorize]
        [UseOffsetPaging(DefaultPageSize = 15)]
        public async Task<IQueryable<ChatEntity>> GetUserChatByChatId([Required] string chatId, ChatType? chatType)
        {
            var sessionId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
            var User = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), sessionId);
            if (User is UserEntity)
            {
                UserEntity userchat = await _dbContext.GetUserCollection().TryGetUserEntityByUsername(chatId);
                switch (chatType)
                {
                    case ChatType.Regular:
                        if (userchat is UserEntity)
                        {
                            return (await GetUserChats()).Where(o => chatType == o.Type).Where(o => userchat.Id != User.Id && o.Participants.Contains(DbRefFactory.UserRef(userchat.Id)));
                        }
                        else return Enumerable.Empty<ChatEntity>().AsQueryable();

                    case ChatType.Group:
                        return (await GetUserChats()).Where(o => chatType == o.Type).Where(o => o.ChatId == chatId);

                    default:
                        if (userchat is UserEntity) return (await GetUserChats()).Where(o => o.Type == ChatType.Regular).Where(o => userchat.Id != User.Id && o.Participants.Contains(DbRefFactory.UserRef(userchat.Id))); // if chatId is a user
                        return (await GetUserChats()).Where(o => o.ChatId == chatId && o.Type == ChatType.Group); // if chatId is a group
                }
            }
            else
            {
                throw new ApplicationException("Session Expired");
            }
        }
    }
}
