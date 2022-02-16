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
using TextAppData.DataEntities;
using TextAppData.Helpers;
using TextAppData.Models;
using TextAppApi.Core;
using TextAppData.ResponseModels;
using TextAppData.Enums;

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
                    // Create output json with last message and sender basic info
                    var messageFromRef_unsafe = await _dbContext.FetchDBRefAsAsync<MessageEntity>(cEnt.Messages.LastOrDefault());
                    var userFromRef_unsafe = await _dbContext.FetchDBRefAsAsync<UserEntity>(messageFromRef_unsafe.Sender); // contains passwords etc!
                    UserResponseModel userResponse = new UserResponseModel {
                        Username = userFromRef_unsafe.Username,
                        FirstName = userFromRef_unsafe.FirstName,
                        LastName = userFromRef_unsafe.LastName
                    };
                    MessageResponseModel message = new MessageResponseModel { 
                        Sender = userResponse,
                        Time = messageFromRef_unsafe.Time,
                        Message = messageFromRef_unsafe.Message
                    };
                    //

                    // Create output json list with participants basic information
                    var participants_unsafe = await _dbContext.FetchDBRefAsAsync<UserEntity>(cEnt.Participants); // contains passwords etc!
                    List<UserResponseModel> participants_filtered = new List<UserResponseModel>();
                    foreach(UserEntity userCurr in participants_unsafe)
                    {
                        participants_filtered.Add(new UserResponseModel { 
                            Username = userCurr.Username,
                            FirstName = userCurr.FirstName,
                            LastName = userCurr.LastName
                        });
                    }
                    //

                    // Combine information from last message and the participants and add it to the list

                    string chatId = cEnt.Type == ChatType.Regular ?
                        (from part in participants_filtered
                         where part.Username != user.Username
                         select part).FirstOrDefault()?.Username : cEnt.GroupId.ToString();

                    chatsResponse.Add(new ChatResponseModel {
                        ChatId = chatId ?? user.Username, // if chatId is null, the user sent the message to himself.
                        Name = cEnt.Name,
                        Type = cEnt.Type,
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
