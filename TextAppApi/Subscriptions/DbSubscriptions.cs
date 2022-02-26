using HotChocolate;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppApi.Authentication;
using TextAppData.DataContext;

namespace TextAppApi.Subscriptions
{
    public partial class DbSubscriptions
    {
        private readonly IDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly ITopicEventReceiver _topicEventReceiver;

        public DbSubscriptions([Service] IDbContext dbContext, [Service] IHttpContextAccessor httpContextAccessor, [Service] ITokenService tokenService, [Service] ITopicEventReceiver topicEventReceiver)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _topicEventReceiver = topicEventReceiver;
        }
    }
}
