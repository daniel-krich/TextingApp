using HotChocolate;
using HotChocolate.Subscriptions;
using HotChocolate.Types;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppApi.Authentication;
using TextAppData.DataContext;
using TextAppData.DataEntities;

namespace TextAppApi.Subscriptions
{
    public partial class DbSubscriptions
    {
        [Subscribe, Topic]
        public IEnumerable<ChatEntity> ListenChatUpdates([EventMessage] ChatEntity chat)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber);
            if (userId is not null && chat.Participants.Contains(DbRefFactory.UserRef(ObjectId.Parse(userId))))
                yield return chat;
        }
    }
}
