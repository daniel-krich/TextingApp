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
                return "invalid token";
            }
        }

        [HttpPost("push")]
        public async Task<string> PushMessage([FromBody] PushMessageModel message)
        {
            var res = await _dbContext.TryGetUserEntityBySessionToken(message.Token);
            if (res is UserEntity user)
            {
                if (message.ChatId.Length > 0)
                {
                    var associatedChat = await _dbContext.GetChatCollection().FindAsync(o => o.Id == new ObjectId(message.ChatId) && o.Participants.Contains(DbRefFactory.UserRef(user.Id)));
                    if (await associatedChat.FirstOrDefaultAsync() is ChatEntity chat)
                    {
                        MessageEntity currentMessage = new MessageEntity
                        {
                            Sender = DbRefFactory.UserRef(user.Id),
                            Time = DateTime.Now,
                            Message = message.Message
                        };
                        await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage);
                        await _dbContext.GetChatCollection().UpdateOneAsync(c => c.Id == chat.Id,
                            Builders<ChatEntity>.Update.AddToSet(o => o.Messages, DbRefFactory.MessageRef(currentMessage.Id)));
                        return "chat should be updated...";
                    }
                    else
                    {
                        return "chat is not found!";
                    }
                }
                else
                {
                    MessageEntity currentMessage = new MessageEntity
                    {
                        Sender = DbRefFactory.UserRef(user.Id),
                        Time = DateTime.Now,
                        Message = message.Message
                    };
                    await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage);
                    await _dbContext.GetChatCollection().InsertOneAsync(new ChatEntity {
                        Participants = new List<MongoDBRef>() { DbRefFactory.UserRef(user.Id) },
                        Messages = new List<MongoDBRef>() { DbRefFactory.MessageRef(currentMessage.Id) }
                    });
                    return "chat should be created...";
                }
            }
            else
            {
                return "invalid token";
            }
        }

        /*[HttpPost("pull")]
        public async Task<string> RetrieveMessageEvent()
        {

        }*/

        [HttpGet("sendmessage")]
        public string SendEvt(string text, string obj_id)
        {
            MessageEvent mEvt = new MessageEvent
            {
                Chat = new ChatEntity
                {
                    Id = ObjectId.GenerateNewId(),
                    Participants = new List<MongoDBRef>() { DbRefFactory.UserRef(new ObjectId(obj_id)) },
                    Messages = new List<MongoDBRef>()
                },
                Message = new MessageEntity
                {
                    Id = ObjectId.GenerateNewId(),
                    Sender = DbRefFactory.UserRef(ObjectId.GenerateNewId()),
                    Time = DateTime.Now,
                    Message = text
                }
            };
            LongPolling<MessageEvent>.CallEvent(mEvt);
            return $"message {text} sent";
        }

        [HttpGet("getmessage")]
        public async Task<string> GetEvt(string obj_id)
        {
            ObjectId myid = new ObjectId(obj_id);
            LongPolling<MessageEvent> e = new LongPolling<MessageEvent>(o => o.Chat.Participants.Where(oo => oo.Id == myid).FirstOrDefault() != null, 30000);
            MessageEvent mE = await e.ResolveEvent();
            if(mE is not null)
            {
                return mE.Message.Message;
            }
            return $"event timeout...";
        }
    }
}
