using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
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
using TextAppApi.Core;
using TextAppData.ResponseModels;

namespace TextAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IDbContext _dbContext;
        public ChatController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<string> GetChats([FromBody] TokenLoginModel token)
        {
            var res = await _dbContext.TryGetUserEntityBySessionToken(token.Token);
            if(res is UserEntity user)
            {
                var associatedChats = await _dbContext.GetChatCollection().FindAsync(o => o.Participants.Contains(DbRefFactory.UserRef(user.Id)));
                List<ChatEntity> chats = await associatedChats.ToListAsync();
                List<ChatResponseModel> chatsResponse = new List<ChatResponseModel>();
                foreach(ChatEntity cEnt in chats)
                {
                    var participants_unfiltered = await _dbContext.FetchDBRefAsAsync<UserEntity>(cEnt.Participants);
                    var messageFromRef = await _dbContext.FetchDBRefAsAsync<MessageEntity>(cEnt.Messages.LastOrDefault());
                    var userFromRef = await _dbContext.FetchDBRefAsAsync<UserEntity>(messageFromRef.Sender);
                    UserResponseModel userResponse = new UserResponseModel {
                        Username = userFromRef.Username,
                        FirstName = userFromRef.FirstName,
                        LastName = userFromRef.LastName
                    };
                    MessageResponseModel message = new MessageResponseModel { 
                        Sender = userResponse,
                        Time = messageFromRef.Time,
                        Message = messageFromRef.Message
                    };

                    List<UserResponseModel> participants_filtered = new List<UserResponseModel>();
                    foreach(UserEntity userCurr in participants_unfiltered)
                    {
                        participants_filtered.Add(new UserResponseModel { 
                            Username = userCurr.Username,
                            FirstName = userCurr.FirstName,
                            LastName = userCurr.LastName
                        });
                    }
                    chatsResponse.Add(new ChatResponseModel {
                        Participants = participants_filtered,
                        LastMessage = message
                    });
                }
                return JsonConvert.SerializeObject(chatsResponse);
            }
            else
            {
                return new ResponseModel(21, "Chat error", "Provided invalid token").ToString();
            }
        }
    }
}
