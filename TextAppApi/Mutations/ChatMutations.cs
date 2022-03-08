using HotChocolate.AspNetCore.Authorization;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TextAppApi.Subscriptions;
using TextAppData.Converters;
using TextAppData.DataContext;
using TextAppData.Factories;
using TextAppData.DataEntities;
using TextAppData.Enums;
using TextAppData.Extensions;
using TextAppData.DTOs.Request;
using TextAppData.DTOs.Response;

namespace TextAppApi.Mutations
{
    public partial class DbMutations
    {
        [Authorize]
        public async Task<ChatEntity> AddMessageToChat(PushMessageDto message)
        {
            if (ModelValidator.Validate(message))
            {
                var sessionId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Sid);
                var User = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), sessionId);
                if (User is UserEntity)
                {

                    if (message.TypeChat == ChatType.Regular)
                    {
                        var finduser = await _dbContext.GetUserCollection().TryGetUserEntityByUsername(message.ChatId);
                        if (finduser is UserEntity founduser && founduser.Id != User.Id) // can't message to yourself
                        {
                            var associatedChat = await _dbContext.GetChatCollection().FindAsync(o => o.Type == ChatType.Regular &&
                                                                    o.Participants.Contains(DbRefFactory.UserRef(founduser.Id)) &&
                                                                    o.Participants.Contains(DbRefFactory.UserRef(User.Id)));
                            if (await associatedChat.FirstOrDefaultAsync() is ChatEntity chat)
                            {
                                MessageEntity currentMessage = new MessageEntity
                                {
                                    Sender = DbRefFactory.UserRef(User.Id),
                                    Time = DateTime.Now,
                                    Message = message.Message
                                };
                                await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage);
                                await _dbContext.GetChatCollection().UpdateOneAsync(c => c.Id == chat.Id,
                                    Builders<ChatEntity>.Update
                                    .AddToSet(o => o.Messages, DbRefFactory.MessageRef(currentMessage.Id))
                                    .Set(o => o.LastActivity, DateTime.Now));
                                var currentChat = (await _dbContext.GetChatCollection().FindAsync(c => c.Id == chat.Id)).FirstOrDefault();

                                // Send new chat event to the participants over ws
                                foreach (var participant in currentChat.Participants)
                                    await _topicEventSender.SendAsync($"{nameof(DbSubscriptions.ListenChatUpdates)}_{participant.Id}", currentChat);
                                //

                                return currentChat;
                            }
                            else
                            {
                                MessageEntity currentMessage = new MessageEntity
                                {
                                    Sender = DbRefFactory.UserRef(User.Id),
                                    Time = DateTime.Now,
                                    Message = message.Message
                                };
                                await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage); // insert first, then we can use the ref with the id.
                                ChatEntity currentChat = new ChatEntity
                                {
                                    LastActivity = DateTime.Now,
                                    Participants = new List<MongoDBRef>() { DbRefFactory.UserRef(User.Id), DbRefFactory.UserRef(founduser.Id) },
                                    Messages = new List<MongoDBRef>() { DbRefFactory.MessageRef(currentMessage.Id) }
                                };
                                await _dbContext.GetChatCollection().InsertOneAsync(currentChat);

                                // Send new chat event to the participants over ws
                                foreach (var participant in currentChat.Participants)
                                    await _topicEventSender.SendAsync($"{nameof(DbSubscriptions.ListenChatUpdates)}_{participant.Id}", currentChat);
                                //

                                return currentChat;
                            }
                        }
                        else
                        {
                            throw new ApplicationException("Couldn't find the user, or you're trying to message yourself.");
                        }
                    }
                    else if (message.TypeChat == ChatType.Group)
                    {
                        var associatedChat = await _dbContext.GetChatCollection().FindAsync(c => c.ChatId == message.ChatId &&
                                                                        c.Participants.Contains(DbRefFactory.UserRef(User.Id)));
                        if (await associatedChat.FirstOrDefaultAsync() is ChatEntity chat)
                        {
                            MessageEntity currentMessage = new MessageEntity
                            {
                                Sender = DbRefFactory.UserRef(User.Id),
                                Time = DateTime.Now,
                                Message = message.Message
                            };
                            await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage);
                            await _dbContext.GetChatCollection().UpdateOneAsync(c => c.Id == chat.Id,
                                Builders<ChatEntity>.Update
                                .AddToSet(o => o.Messages, DbRefFactory.MessageRef(currentMessage.Id))
                                .Set(o => o.LastActivity, DateTime.Now));
                            var currentChat = (await _dbContext.GetChatCollection().FindAsync(c => c.Id == chat.Id)).FirstOrDefault();

                            // Send new chat event to the participants over ws
                            foreach (var participant in currentChat.Participants)
                                await _topicEventSender.SendAsync($"{nameof(DbSubscriptions.ListenChatUpdates)}_{participant.Id}", currentChat);
                            //

                            return currentChat;
                        }
                        else
                        {
                            throw new ApplicationException("Group chat wasn't found.");
                        }
                    }
                    else
                    {
                        throw new ApplicationException("Unknown chat type.");
                    }
                }
                else
                {
                    throw new ApplicationException("Session Expired");
                }
            }
            else
            {
                throw new ApplicationException("Parameters didn't pass validation, re-check fields.");
            }
        }
    }
}
