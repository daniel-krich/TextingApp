using HotChocolate;
using HotChocolate.Execution;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppApi.Authentication;
using TextAppApi.Core;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using TextAppData.Helpers;

namespace TextAppApi.Subscriptions
{
    public partial class DbSubscriptions
    {
        [SubscribeAndResolve]
        public async ValueTask<ISourceStream<ChatEntity>> ListenChatUpdates()
        {
            try
            {
                var sessionId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
                var User = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), sessionId);
                if (User is UserEntity)
                {
                    return await _topicEventReceiver.SubscribeAsync<string, ChatEntity>($"{nameof(ListenChatUpdates)}_{User.Id}");
                }
                else
                {
                    return default;
                }
            }
            catch
            {
                return default;
            }
        }
    }
}
