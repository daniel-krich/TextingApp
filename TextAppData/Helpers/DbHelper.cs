using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextAppData.DataContext;
using TextAppData.DataEntities;

namespace TextAppData.Helpers
{
    public static class DbHelper
    {
        private static string TokenCharacters { get => "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_!-"; }
        private static Random Random = new Random();
        public static async Task<string> CreateRandomSessionToken(IMongoCollection<SessionEntity> session, UserEntity user, int length)
        {
            StringBuilder token = new StringBuilder();
            do
            {
                for (int i = 0; i < length; i++)
                {
                    token.Append(TokenCharacters[Random.Next(TokenCharacters.Length)]);
                }
                if(!await (await session.FindAsync(o => o.Token == token.ToString())).AnyAsync())
                {
                    await session.InsertOneAsync(new SessionEntity { 
                        Token = token.ToString(),
                        ExpiresAt = DateTime.Now,
                        User = DbRefFactory.UserRef(user.Id)
                    });
                    return token.ToString();
                }
                else token.Clear();

            } while (true);
            
        }
    }
}
