﻿using HotChocolate.AspNetCore.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using TextAppData.Enums;


namespace TextAppApi.Queries
{
    public partial class DbQueries
    {
        [Authorize]
        public UserEntity GetUser()
        {
            return _dbContext.GetUserCollection()
                    .AsQueryable()
                    .Where(o => o.Id == ObjectId.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber))).FirstOrDefault();
        }
    }
}
