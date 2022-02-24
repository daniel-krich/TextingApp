using HotChocolate;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.DataContext;

namespace TextAppApi.Mutations
{
    public partial class DbMutations
    {
        private readonly IDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DbMutations([Service] IDbContext dbContext, [Service] IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
