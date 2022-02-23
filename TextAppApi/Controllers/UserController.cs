using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppApi.Authentication;
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
        private ITokenService _tokenService;
        public UserController(IDbContext dbContext, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenService = tokenService;
        }

        [HttpGet("refresh")]
        [Authorize]
        public async Task<string> AuthByToken()
        {
            var res = await _dbContext.TryGetUserEntityById(this.User.FindFirstValue(ClaimTypes.SerialNumber));
            if (res is UserEntity model)
            {
                return ModelConverter.Convert<AuthWithTokenResponseModel>(model).ToString();
            }
            else
            {
                return new ResponseModel(1, "Authentication error", "Invalid token").ToString();
            }
        }

        [HttpPost("login")]
        public async Task<string> AuthByUsernamePass([FromBody] LoginModel user)
        {
            var res = await _dbContext.TryGetUserEntityByCredentials(user.Username, user.Password);
            if (res is UserEntity model)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.SerialNumber, model.Id.ToString()),
                    new Claim(ClaimTypes.Name, model.Username)
                };

                var accessToken = _tokenService.GenerateAccessToken(claims);

                return new AuthResponseModel {
                    AccessToken = accessToken
                }.ToString();
            }
            else
            {
                return new ResponseModel(1, "Authentication error", "Invalid username or password").ToString();
            }
        }


        [HttpPost("register")]
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
        [Authorize]
        public async Task<string> SearchUsers([FromBody] SearchUsersModel search)
        {

            var res = await _dbContext.TryGetUserEntityById(this.User.FindFirstValue(ClaimTypes.SerialNumber));
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

        [HttpPost("find")]
        [Authorize]
        public async Task<string> FindUser([FromBody] SearchUsersModel search)
        {

            var res = await _dbContext.TryGetUserEntityById(this.User.FindFirstValue(ClaimTypes.SerialNumber));
            if (res is UserEntity user)
            {
                var userRes = await _dbContext.TryGetUserEntityByUsername(search.Query);
                if (userRes is UserEntity foundUser)
                {
                    UserResponseModel userResponseModel = new UserResponseModel()
                    {
                        Username = foundUser.Username,
                        FirstName = foundUser.FirstName,
                        LastName = foundUser.LastName
                    };
                    return userResponseModel.ToString();
                }
                else
                {
                    return new ResponseModel(1, "Search error", "Provided invalid username.").ToString();
                }
            }
            else
            {
                return new ResponseModel(1, "Search error", "Provided invalid token.").ToString();
            }
        }
    }
}
