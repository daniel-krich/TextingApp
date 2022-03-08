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
using TextAppData.Converters;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using TextAppData.Enums;
using TextAppData.Extensions;
using TextAppData.DTOs.Request;
using TextAppData.DTOs.Response;

namespace TextAppApi.Queries
{
    public partial class DbQueries
    {
        [Authorize]
        public async Task<UserEntity> GetUser()
        {
            var sessionId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
            var User = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), sessionId);
            if (User is UserEntity)
            {
                return User;
            }
            else
            {
                throw new ApplicationException("Session Expired");
            }
        }

        public async Task<string> Login(LoginDto login)
        {
            if (ModelValidator.Validate(login))
            {
                var res = await _dbContext.GetUserCollection().TryGetUserEntityByCredentials(login.Username, login.Password);
                if (res is UserEntity model)
                {
                    var sessionId = await _dbContext.GetSessionCollection().CreateSessionId(model);
                    return _tokenService.GenerateAccessToken(new List<Claim>
                    {
                        new Claim(ClaimTypes.Sid, sessionId)
                    });
                }
                else
                {
                    throw new ApplicationException("Invalid username or password");
                }
            }
            else
            {
                throw new ApplicationException("Parameters didn't pass validation, re-check fields.");
            }
        }

        [Authorize]
        [UseOffsetPaging(DefaultPageSize = 50)]
        public async Task<IQueryable<UserEntity>> SearchUser([Required] string name, [Required] bool exact)
        {
            var sessionId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
            var User = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), sessionId);
            if (User is UserEntity)
            {
                if (exact)
                {
                    return _dbContext.GetUserCollection().AsQueryable().Where(o => o.Username == name);
                }
                else
                {
                    return _dbContext.GetUserCollection().AsQueryable().Where(o => o.Username.Contains(name) || o.FirstName.Contains(name) || o.LastName.Contains(name));
                }
            }
            else
            {
                throw new ApplicationException("Session Expired");
            }
        }
    }
}
