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
using TextAppData.DataEntities;
using TextAppData.Enums;
using TextAppData.Models;

namespace TextAppApi.Mutations
{
    public partial class DbMutations
    {
        [Authorize]
        public async Task<ChatEntity> AddMessageToChat(PushMessageModel message)
        {
            if (ModelValidator.Validate(message))
            {
                var res = await _dbContext.TryGetUserEntityById(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.SerialNumber));
                if (res is UserEntity user)
                {

                    if (message.TypeChat == ChatType.Regular)
                    {
                        var finduser = await _dbContext.TryGetUserEntityByUsername(message.ChatId);
                        if (finduser is UserEntity founduser && founduser.Id != user.Id) // can't message to yourself
                        {
                            var associatedChat = await _dbContext.GetChatCollection().FindAsync(o => o.Type == ChatType.Regular &&
                                                                    o.Participants.Contains(DbRefFactory.UserRef(founduser.Id)) &&
                                                                    o.Participants.Contains(DbRefFactory.UserRef(user.Id)));
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
                                    Sender = DbRefFactory.UserRef(user.Id),
                                    Time = DateTime.Now,
                                    Message = message.Message
                                };
                                await _dbContext.GetMessageCollection().InsertOneAsync(currentMessage); // insert first, then we can use the ref with the id.
                                ChatEntity currentChat = new ChatEntity
                                {
                                    Participants = new List<MongoDBRef>() { DbRefFactory.UserRef(user.Id), DbRefFactory.UserRef(founduser.Id) },
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
                                                                        c.Participants.Contains(DbRefFactory.UserRef(user.Id)));
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
                    throw new ApplicationException("Invalid token provided.");
                }
            }
            else
            {
                throw new ApplicationException("Parameters didn't pass validation, re-check fields.");
            }
        }
    }
}
