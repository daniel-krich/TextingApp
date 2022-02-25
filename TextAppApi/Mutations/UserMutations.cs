using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.Converters;
using TextAppData.DataEntities;
using TextAppData.Models;

namespace TextAppApi.Mutations
{
    public partial class DbMutations
    {
        public async Task<UserEntity> CreateUser(CreateUserModel createUser)
        {
            if (ModelValidator.Validate(createUser))
            {
                UserEntity user = new UserEntity
                {
                    Username = createUser.Username,
                    Password = createUser.Password,
                    Email = createUser.Email,
                    FirstName = createUser.FirstName,
                    LastName = createUser.LastName
                };
                try
                {
                    await _dbContext.GetUserCollection().InsertOneAsync(user);
                    return user;
                }
                catch(MongoWriteException)
                {
                    throw new ApplicationException("Username or email already exists.");
                }
            }
            else
            {
                throw new ApplicationException("Parameters didn't pass validation, re-check fields.");
            }
        }
    }
}
