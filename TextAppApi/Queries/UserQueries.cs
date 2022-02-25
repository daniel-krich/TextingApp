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
using TextAppData.Models;

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

        public async Task<string> Login(LoginModel login)
        {
            if (ModelValidator.Validate(login))
            {
                var res = await _dbContext.TryGetUserEntityByCredentials(login.Username, login.Password);
                if (res is UserEntity model)
                {
                    return _tokenService.GenerateAccessToken(new List<Claim>
                    {
                        new Claim(ClaimTypes.SerialNumber, model.Id.ToString()),
                        new Claim(ClaimTypes.Name, model.Username)
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
        public IQueryable<UserEntity> SearchUser([Required] string name, [Required] bool exact)
        {
            if(exact)
            {
                return _dbContext.GetUserCollection().AsQueryable().Where(o => o.Username == name);
            }
            else
            {
                return _dbContext.GetUserCollection().AsQueryable().Where(o => o.Username.Contains(name) || o.FirstName.Contains(name) || o.LastName.Contains(name));
            }
        }
    }
}
