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
using TextAppData.DataEntities;
using TextAppData.Helpers;
using TextAppData.Models;
using TextAppData.ResponseModels;

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
                return ModelConverter.Convert<AuthWithTokenResponseModel>(model).ToString();
            }
            else
            {
                return new ResponseModel(1, "Authentication error", "Invalid token").ToString();
            }
        }

        [HttpPost("signin")]
        public async Task<string> AuthByUsernamePass([FromBody] LoginModel user)
        {
            var res = await _dbContext.TryGetUserEntityByCredentials(user.Username, user.Password);
            if (res is UserEntity model)
            {
                var token = await DbHelper.CreateRandomSessionToken(_dbContext.GetSessionCollection(), model, 60);
                return new AuthResponseModel { Token = token }.ToString();
            }
            else
            {
                return new ResponseModel(1, "Authentication error", "Invalid username or password").ToString();
            }
        }


        [HttpPost("signup")]
        public async Task<string> CreateUser([FromBody] CreateUserModel user)
        {
            try
            {
                await _dbContext.GetUserCollection().InsertOneAsync(ModelConverter.Convert<UserEntity>(user));
                return new ResponseModel("Success", "Account has been created").ToString();
            }
            catch(MongoWriteException)
            {
                return new ResponseModel(2, "Signup error", "Couldn't insert a new account, some of the data exists already.").ToString();
            }
            catch(Exception)
            {
                return new ResponseModel(0, "Unknown error", "Error occured on create user.").ToString();
            }
        }

        [HttpPost("search")]
        public async Task<string> SearchUsers([FromBody] SearchUsersModel search)
        {
            var res = await _dbContext.TryGetUserEntityBySessionToken(search.Token);
            if (res is UserEntity user)
            {
                var usersResult = (await _dbContext.GetUserCollection().FindAsync(o => o.Username.ToLower().Contains(search.Query.ToLower()) || o.FirstName.ToLower().Contains(search.Query.ToLower()) || o.LastName.ToLower().Contains(search.Query.ToLower()))).ToEnumerable().Take(50);
                IList<UserResponseModel> responseUsers = new List<UserResponseModel>();
                foreach (UserEntity userFromDb in usersResult)
                    responseUsers.Add(new UserResponseModel {
                        Username = userFromDb.Username,
                        FirstName = userFromDb.FirstName,
                        LastName = userFromDb.LastName
                    });
                return JsonConvert.SerializeObject(responseUsers);
            }
            else
            {
                return new ResponseModel(1, "Search error", "Provided invalid token.").ToString();
            }
        }
    }
}
