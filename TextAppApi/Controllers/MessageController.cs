using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppApi.Core;
using TextAppData.DataContext;
using TextAppData.Factories;
using TextAppData.DataEntities;
using TextAppData.Enums;
using TextAppData.Extensions;
using TextAppData.DTOs.Request;
using TextAppData.DTOs.Response;

namespace TextAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        //private IDbContext _dbContext;

        //public MessageController(IDbContext dbContext)
        //{
        //    _dbContext = dbContext;
        //}

        //[HttpPost("push")]
        //[Authorize]
        //public async Task<string> PushMessage([FromBody] PushMessageDto message)
        //{
        //    var res = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), this.User.FindFirstValue(ClaimTypes.Sid));
        //    if (res is UserEntity user)
        //    {
        //        if (message.ChatId.Length > 0)
        //        {
        //            if (message.TypeChat == ChatType.Regular)
        //            {
        //                var finduser = await _dbContext.GetUserCollection().TryGetUserEntityByUsername(message.ChatId);
        //                if (finduser is UserEntity founduser && founduser.Id != user.Id) // can't message to yourself
        //                {
        //                    var associatedChat = await _dbContext.GetChatCollection().FindAsync(o => o.Type == ChatType.Regular && 
        //                                                            o.Participants.Contains(DbRefFactory.UserRef(founduser.Id)) &&
        //                                                            o.Participants.Contains(DbRefFactory.UserRef(user.Id)));
        //                    if (await associatedChat.FirstOrDefaultAsync() is ChatEntity chat)
        //                    {
        //                        MessageEntity currentMessage = new MessageEntity
        //                        {
        //                            Sender = DbRefFactory.UserRef(user.Id),
        //                            Time = DateTime.Now,
        //                            Message = message.Message
        //                        };
        //                        await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage);
        //                        await _dbContext.GetChatCollection().UpdateOneAsync(c => c.Id == chat.Id,
        //                            Builders<ChatEntity>.Update.AddToSet(o => o.Messages, DbRefFactory.MessageRef(currentMessage.Id)));
        //                        EventStream<MessageEvent>.CallEvent(new MessageEvent { Message = currentMessage, Chat = chat });
        //                        return new ResponseDto("Success", "Message sent successfully").ToString();
        //                    }
        //                    else
        //                    {
        //                        MessageEntity currentMessage = new MessageEntity
        //                        {
        //                            Sender = DbRefFactory.UserRef(user.Id),
        //                            Time = DateTime.Now,
        //                            Message = message.Message
        //                        };
        //                        await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage); // insert first, then we can use the ref with the id.
        //                        ChatEntity currentChat = new ChatEntity
        //                        {
        //                            Participants = new List<MongoDBRef>() { DbRefFactory.UserRef(user.Id), DbRefFactory.UserRef(founduser.Id) },
        //                            Messages = new List<MongoDBRef>() { DbRefFactory.MessageRef(currentMessage.Id) }
        //                        };
        //                        await _dbContext.GetChatCollection().InsertOneAsync(currentChat);
        //                        EventStream<MessageEvent>.CallEvent(new MessageEvent { Message = currentMessage, Chat = currentChat });
        //                        return new ResponseDto("Success", "New chat has been created, message sent successfully").ToString();
        //                    }
        //                }
        //                else
        //                {
        //                    return new ResponseDto(21, "Message error", "Couldn't find chat or user to send to.").ToString();
        //                }
        //            }
        //            else if(message.TypeChat == ChatType.Group)
        //            {
        //                try
        //                {
        //                    //ulong GroupId = Convert.ToUInt64(message.ChatId);
        //                    var associatedChat = await _dbContext.GetChatCollection().FindAsync(c => c.ChatId == message.ChatId/*GroupId*/ &&
        //                                                                  c.Participants.Contains(DbRefFactory.UserRef(user.Id)));
        //                    if (await associatedChat.FirstOrDefaultAsync() is ChatEntity chat)
        //                    {
        //                        MessageEntity currentMessage = new MessageEntity
        //                        {
        //                            Sender = DbRefFactory.UserRef(user.Id),
        //                            Time = DateTime.Now,
        //                            Message = message.Message
        //                        };
        //                        await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage);
        //                        await _dbContext.GetChatCollection().UpdateOneAsync(c => c.Id == chat.Id,
        //                            Builders<ChatEntity>.Update.AddToSet(o => o.Messages, DbRefFactory.MessageRef(currentMessage.Id)));
        //                        EventStream<MessageEvent>.CallEvent(new MessageEvent { Message = currentMessage, Chat = chat });
        //                        return new ResponseDto("Success", "Message sent successfully").ToString();
        //                    }
        //                    else
        //                    {
        //                        return new ResponseDto(24, "Message error", "Couldn't find chat or user to send to.").ToString();
        //                    }
        //                }
        //                catch(OverflowException)
        //                {
        //                    return new ResponseDto(22, "Id error", "Provided invalid Id").ToString();
        //                }
        //                catch(FormatException)
        //                {
        //                    return new ResponseDto(23, "Id error", "Provided invalid Id").ToString();
        //                }
        //                catch(Exception)
        //                {
        //                    return new ResponseDto(0, "Unknown error", "Error occured on push message.").ToString();
        //                }
        //            }
        //            else
        //            {
        //                return new ResponseDto(21, "Message error", "Couldn't find chat or user to send to.").ToString();
        //            }
        //        }
        //        else
        //        {
        //            return new ResponseDto(23, "Id error", "Provided invalid Id").ToString();
        //        }
        //    }
        //    else
        //    {
        //        return new ResponseDto(21, "Message error", "Provided invalid token").ToString();
        //    }
        //}

        //[HttpGet("pull")]
        //[Authorize]
        //public async Task RetrieveMessageEvent()
        //{
        //    Response.ContentType = "text/event-stream";
        //    var res = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), this.User.FindFirstValue(ClaimTypes.Sid));
        //    if (res is UserEntity user)
        //    {
        //        do
        //        {
        //            EventStream<MessageEvent> eventPolling = new EventStream<MessageEvent>(o => o.Chat.Participants.Where(oo => oo.Id == user.Id).FirstOrDefault() is not null);
        //            MessageEvent messageEvent = await eventPolling.ResolveEvent();
        //            if (messageEvent is not null)
        //            {
        //                // Create output json with last message and sender basic info
        //                var userFromRef_unsafe = await _dbContext.GetUserCollection().FetchDBRefAsAsync(messageEvent.Message.Sender); // contains passwords etc!
        //                UserResponseDto userResponse = new UserResponseDto
        //                {
        //                    Username = userFromRef_unsafe.Username,
        //                    FirstName = userFromRef_unsafe.FirstName,
        //                    LastName = userFromRef_unsafe.LastName
        //                };
        //                MessageResponseDto message = new MessageResponseDto
        //                {
        //                    Sender = userResponse,
        //                    Time = messageEvent.Message.Time,
        //                    Message = messageEvent.Message.Message
        //                };
        //                //

        //                // Create output json list with participants basic information
        //                var participants_unsafe = await _dbContext.GetUserCollection().FetchDBRefAsAsync(messageEvent.Chat.Participants); // contains passwords etc!
        //                List<UserResponseDto> participants_filtered = new List<UserResponseDto>();
        //                foreach (UserEntity userCurr in participants_unsafe)
        //                {
        //                    participants_filtered.Add(new UserResponseDto
        //                    {
        //                        Username = userCurr.Username,
        //                        FirstName = userCurr.FirstName,
        //                        LastName = userCurr.LastName
        //                    });
        //                }
        //                //

        //                // Combine information from last message and the participants and add it to the list

        //                string chatId = messageEvent.Chat.Type == ChatType.Regular ?
        //                    (from part in participants_filtered
        //                     where part.Username != user.Username
        //                     select part).FirstOrDefault()?.Username : messageEvent.Chat.ChatId.ToString();

        //                ChatResponseDto responseChat = new ChatResponseDto
        //                {
        //                    ChatId = chatId ?? user.Username, // if chatId is null, the user sent the message to himself.
        //                    Name = messageEvent.Chat.Name,
        //                    Type = messageEvent.Chat.Type,
        //                    Participants = participants_filtered,
        //                    LastMessage = message
        //                };
        //                await Response.WriteAsync($"data: {responseChat}\n\n");
        //            }
        //        } while (!Request.HttpContext.RequestAborted.IsCancellationRequested);
        //    }
        //    else
        //    {
        //        Response.StatusCode = 401;
        //    }
        //}
    }
}
