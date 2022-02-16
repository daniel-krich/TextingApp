﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TextAppApi.Core;
using TextAppData.DataContext;
using TextAppData.DataModels;
using TextAppData.Models;
using TextAppData.ResponseModels;

namespace TextAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private IDbContext _dbContext;

        public MessageController(IDbContext dbContext)
        {
            _dbContext = dbContext;
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
                        LongPolling<MessageEvent>.CallEvent( new MessageEvent { Message = currentMessage, Chat = chat });
                        return new ResponseModel("Success", "Message sent successfully").ToString();
                    }
                    else
                    {
                        return new ResponseModel(20, "Message error", "Chat not found").ToString();
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
                    ChatEntity currentChat = new ChatEntity
                    {
                        Participants = new List<MongoDBRef>() { DbRefFactory.UserRef(user.Id) },
                        Messages = new List<MongoDBRef>() { DbRefFactory.MessageRef(currentMessage.Id) }
                    };
                    await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage);
                    await _dbContext.GetChatCollection().InsertOneAsync(currentChat);
                    LongPolling<MessageEvent>.CallEvent(new MessageEvent { Message = currentMessage, Chat = currentChat });
                    return new ResponseModel("Success", "New chat has been created, message sent successfully").ToString();
                }
            }
            else
            {
                return new ResponseModel(21, "Message error", "Provided invalid token").ToString();
            }
        }

        [HttpGet("pull/{token}")]
        public async Task RetrieveMessageEvent(string token)
        {
            Response.ContentType = "text/event-stream";
            var res = await _dbContext.TryGetUserEntityBySessionToken(token);
            if (res is UserEntity user)
            {
                do
                {
                    LongPolling<MessageEvent> eventPolling = new LongPolling<MessageEvent>(o => o.Chat.Participants.Where(oo => oo.Id == user.Id).FirstOrDefault() != null);
                    MessageEvent messageEvent = await eventPolling.ResolveEvent();
                    if (messageEvent is not null)
                    {
                        await Response.WriteAsync($"data: {messageEvent.Message.Message}\n\n");
                    }
                } while (!Request.HttpContext.RequestAborted.IsCancellationRequested);
            }
            else
            {
                Response.StatusCode = 401;
            }
        }
    }
}