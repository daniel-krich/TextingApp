using HotChocolate;
using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppApi.Authentication;
using TextAppData.DataContext;

namespace TextAppApi.Mutations
{
    public partial class DbMutations
    {
        private readonly IDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        private readonly ITopicEventSender _topicEventSender;

        public DbMutations([Service] IDbContext dbContext, [Service] IHttpContextAccessor httpContextAccessor, [Service] ITokenService tokenService, [Service] ITopicEventSender topicEventSender)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
            _topicEventSender = topicEventSender;
        }
    }
}
