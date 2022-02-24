using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.DataEntities;

namespace TextAppApi.Mutations
{
    public partial class DbMutations
    {
        public async Task<UserEntity> CreateUser(
            [Required(ErrorMessage = "Username is required"), MinLength(4, ErrorMessage = "Min length of username is 4"), MaxLength(24, ErrorMessage = "Max length of username is 24")]
            string username,
            [Required(ErrorMessage = "password is required"), MinLength(6, ErrorMessage = "Min length of password is 6"), MaxLength(24, ErrorMessage = "Max length of password is 24")]
            string password,
            [Required(ErrorMessage = "firstname is required"), MinLength(1, ErrorMessage = "Min length of firstname is 1"), MaxLength(30, ErrorMessage = "Max length of firstname is 30")]
            string firstname,
            [Required(ErrorMessage = "lastname is required"), MinLength(1, ErrorMessage = "Min length of lastname is 1"), MaxLength(30, ErrorMessage = "Max length of lastname is 30")]
            string lastname,
            [Required(ErrorMessage = "email is required"), MinLength(10, ErrorMessage = "Min length of email is 10"), MaxLength(60, ErrorMessage = "Max length of email is 60")]
            string email
        )
        {
            
            UserEntity user = new UserEntity
            {
                Username = username,
                Password = password,
                Email = email,
                FirstName = firstname,
                LastName = lastname
            };
            await _dbContext.GetUserCollection().InsertOneAsync(user);
            return user;
        }
    }
}
