using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.Converters;
using TextAppData.DataContext;
using TextAppData.DataModels;
using TextAppData.Helpers;
using TextAppData.Models;

namespace TextAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IDbContext _dbContext;
        public UserController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("auth_token")]
        public async Task<string> AuthByToken([FromBody] TokenLoginModel user)
        {
            var res = await _dbContext.TryGetUserEntityBySessionToken(user.Token);
            if (res is UserEntity model)
            {
                return JsonConvert.SerializeObject(model);
            }
            else
            {
                return "invalid token";
            }
        }

        [HttpPost("signin")]
        public async Task<string> AuthByUsernamePass([FromBody] LoginModel user)
        {
            var res = await _dbContext.TryGetUserEntityByCredentials(user.Username, user.Password);
            if (res is UserEntity model)
            {
                var token = await DbHelper.CreateRandomSessionToken(_dbContext.GetSessionCollection(), model, 60);
                return JsonConvert.SerializeObject(model);
            }
            else
            {
                return "not found";
            }
        }


        [HttpPost("signup")]
        public async Task<string> CreateUser([FromBody] CreateUserModel user)
        {
            try
            {
                await _dbContext.GetUserCollection().InsertOneAsync(ModelConverter.Convert<UserEntity>(user));
                return "Success";
            }
            catch(MongoWriteException err)
            {
                return err.Message;
            }
        }
    }
}
