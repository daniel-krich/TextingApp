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
using HotChocolate.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TextAppData.Enums;
using System.Diagnostics;
using TextAppApi.Authentication;

namespace TextAppApi.Queries
{
    public partial class DbQueries
    {
        private readonly IDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITokenService _tokenService;
        public DbQueries([Service] IDbContext dbContext, [Service] IHttpContextAccessor httpContextAccessor, [Service] ITokenService tokenService)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _tokenService = tokenService;
        }
    }
}
