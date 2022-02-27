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

namespace TextAppApi.Subscriptions
{
    public partial class DbSubscriptions
    {

        [SubscribeAndResolve]
        public ValueTask<ISourceStream<ChatEntity>> ListenChatUpdates()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber);
            if (userId is not null)
                return _topicEventReceiver.SubscribeAsync<string, ChatEntity>($"{nameof(ListenChatUpdates)}_{userId}");
            else
                return default;
        }

    }
}
