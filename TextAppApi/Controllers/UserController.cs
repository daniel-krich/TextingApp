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
using TextAppData.Extensions;
using TextAppData.DTOs.Request;
using TextAppData.DTOs.Response;

namespace TextAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        //private IDbContext _dbContext;
        //private ITokenService _tokenService;
        //public UserController(IDbContext dbContext, ITokenService tokenService)
        //{
        //    _dbContext = dbContext;
        //    _tokenService = tokenService;
        //}

        //[HttpGet("refresh")]
        //[Authorize]
        //public async Task<string> AuthByToken()
        //{
        //    var res = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), this.User.FindFirstValue(ClaimTypes.Sid));
        //    if (res is UserEntity model)
        //    {
        //        return ModelConverter.Convert<AuthWithTokenResponseDto>(model).ToString();
        //    }
        //    else
        //    {
        //        return new ResponseDto(1, "Authentication error", "Invalid token").ToString();
        //    }
        //}

        //[HttpPost("login")]
        //public async Task<string> AuthByUsernamePass([FromBody] LoginDto user)
        //{
        //    var res = await _dbContext.GetUserCollection().TryGetUserEntityByCredentials(user.Username, user.Password);
        //    if (res is UserEntity model)
        //    {

        //        var sessionId = await _dbContext.GetSessionCollection().CreateSessionId(model);
        //        var accessToken = _tokenService.GenerateAccessToken(new List<Claim>
        //        {
        //            new Claim(ClaimTypes.Sid, sessionId)
        //        });

        //        return new AuthResponseDto { AccessToken = accessToken }.ToString();
        //    }
        //    else
        //    {
        //        return new ResponseDto(1, "Authentication error", "Invalid username or password").ToString();
        //    }
        //}


        //[HttpPost("register")]
        //public async Task<string> CreateUser([FromBody] CreateUserDto user)
        //{
        //    try
        //    {
        //        await _dbContext.GetUserCollection().InsertOneAsync(ModelConverter.Convert<UserEntity>(user));
        //        return new ResponseDto("Success", "Account has been created").ToString();
        //    }
        //    catch(MongoWriteException)
        //    {
        //        return new ResponseDto(2, "Signup error", "Couldn't insert a new account, some of the data exists already.").ToString();
        //    }
        //    catch(Exception)
        //    {
        //        return new ResponseDto(0, "Unknown error", "Error occured on create user.").ToString();
        //    }
        //}

        //[HttpPost("search")]
        //[Authorize]
        //public async Task<string> SearchUsers([FromBody] SearchUsersDto search)
        //{

        //    var res = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), this.User.FindFirstValue(ClaimTypes.Sid));
        //    if (res is UserEntity user)
        //    {
        //        var usersResult = (await _dbContext.GetUserCollection().FindAsync(o => o.Username.ToLower().Contains(search.Query.ToLower()) || o.FirstName.ToLower().Contains(search.Query.ToLower()) || o.LastName.ToLower().Contains(search.Query.ToLower()))).ToEnumerable().Take(50);
        //        IList<UserResponseDto> responseUsers = new List<UserResponseDto>();
        //        foreach (UserEntity userFromDb in usersResult)
        //            responseUsers.Add(new UserResponseDto {
        //                Username = userFromDb.Username,
        //                FirstName = userFromDb.FirstName,
        //                LastName = userFromDb.LastName
        //            });
        //        return JsonConvert.SerializeObject(responseUsers);
        //    }
        //    else
        //    {
        //        return new ResponseDto(1, "Search error", "Provided invalid token.").ToString();
        //    }
        //}

        //[HttpPost("find")]
        //[Authorize]
        //public async Task<string> FindUser([FromBody] SearchUsersDto search)
        //{

        //    var res = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), this.User.FindFirstValue(ClaimTypes.Sid));
        //    if (res is UserEntity user)
        //    {
        //        var userRes = await _dbContext.GetUserCollection().TryGetUserEntityByUsername(search.Query);
        //        if (userRes is UserEntity foundUser)
        //        {
        //            UserResponseDto userResponseModel = new UserResponseDto()
        //            {
        //                Username = foundUser.Username,
        //                FirstName = foundUser.FirstName,
        //                LastName = foundUser.LastName
        //            };
        //            return userResponseModel.ToString();
        //        }
        //        else
        //        {
        //            return new ResponseDto(1, "Search error", "Provided invalid username.").ToString();
        //        }
        //    }
        //    else
        //    {
        //        return new ResponseDto(1, "Search error", "Provided invalid token.").ToString();
        //    }
        //}
    }
}
