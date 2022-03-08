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
using TextAppData.Factories;
using TextAppData.DataEntities;
using TextAppData.Extensions;
using TextAppData.DTOs.Response;
using TextAppData.DTOs.Request;
using TextAppApi.Core;
using TextAppData.Enums;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TextAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private IDbContext _dbContext;
        private const int AmountOfMessagesAtOnce = 10;
        public ChatController(IDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        [Authorize]
        public async Task<string> GetChats()
        {
            var res = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), this.User.FindFirstValue(ClaimTypes.Sid));
            if (res is UserEntity user)
            {
                var associatedChats = await _dbContext.GetChatCollection().FindAsync(o => o.Participants.Contains(DbRefFactory.UserRef(user.Id)));
                List<ChatEntity> chats = await associatedChats.ToListAsync();
                List<ChatResponseDto> chatsResponse = new List<ChatResponseDto>();
                foreach (ChatEntity cEnt in chats)
                {
                    // Create output json with last message and sender basic info
                    var messageFromRef_unsafe = await _dbContext.GetMessageCollection().FetchDBRefAsAsync(cEnt.Messages.LastOrDefault());
                    var userFromRef_unsafe = await _dbContext.GetUserCollection().FetchDBRefAsAsync(messageFromRef_unsafe.Sender); // contains passwords etc!
                    UserResponseDto userResponse = new UserResponseDto
                    {
                        Username = userFromRef_unsafe.Username,
                        FirstName = userFromRef_unsafe.FirstName,
                        LastName = userFromRef_unsafe.LastName
                    };
                    MessageResponseDto message = new MessageResponseDto
                    {
                        Sender = userResponse,
                        Time = messageFromRef_unsafe.Time,
                        Message = messageFromRef_unsafe.Message
                    };
                    //

                    // Create output json list with participants basic information
                    var participants_unsafe = await _dbContext.GetUserCollection().FetchDBRefAsAsync(cEnt.Participants); // contains passwords etc!
                    List<UserResponseDto> participants_filtered = new List<UserResponseDto>();
                    foreach (UserEntity userCurr in participants_unsafe)
                    {
                        participants_filtered.Add(new UserResponseDto
                        {
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
                         select part).FirstOrDefault()?.Username : cEnt.ChatId.ToString();

                    chatsResponse.Add(new ChatResponseDto
                    {
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
                return new ResponseDto(21, "Chat error", "Provided invalid token").ToString();
            }
        }

        [HttpPost("contact")]
        [Authorize]
        public async Task<string> GetChatContactInfo([FromBody] GetChatContactDto chat)
        {
            var res = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), this.User.FindFirstValue(ClaimTypes.Sid));
            if (res is UserEntity user)
            {
                var finduser = await _dbContext.GetUserCollection().TryGetUserEntityByUsername(chat.ChatId);
                if (finduser is UserEntity founduser)
                {
                    IList<UserResponseDto> userResponses = new List<UserResponseDto>();
                    userResponses.Add(new UserResponseDto
                    {
                        Username = user.Username,
                        FirstName = user.FirstName,
                        LastName = user.LastName
                    });
                    userResponses.Add(new UserResponseDto
                    {
                        Username = founduser.Username,
                        FirstName = founduser.FirstName,
                        LastName = founduser.LastName
                    });
                    ChatResponseDto chatResponse = new ChatResponseDto
                    {
                        ChatId = chat.ChatId,
                        Type = ChatType.Regular,
                        Participants = userResponses
                    };
                    return chatResponse.ToString();
                }
                else
                {
                    return new ResponseDto(21, "Chat error", "Couldn't find user.").ToString();
                }
            }
            else
            {
                return new ResponseDto(21, "Chat error", "Provided invalid token.").ToString();
            }
        }

        [HttpPost("messages")]
        [Authorize]
        public async Task<string> GetChatMessagesOffset([FromBody] GetChatMessagesDto chat)
        {
            var res = await _dbContext.GetSessionCollection().TryGetUserEntityBySessionId(_dbContext.GetUserCollection(), this.User.FindFirstValue(ClaimTypes.Sid));
            if (res is UserEntity user)
            {
                if (chat.ChatId.Length > 0)
                {
                    if (chat.TypeChat == ChatType.Regular)
                    {
                        var finduser = await _dbContext.GetUserCollection().TryGetUserEntityByUsername(chat.ChatId);
                        if (finduser is UserEntity founduser)
                        {
                            var associatedChat = await _dbContext.GetChatCollection().FindAsync(o => o.Type == ChatType.Regular &&
                                                                    o.Participants.Contains(DbRefFactory.UserRef(founduser.Id)) &&
                                                                    o.Participants.Contains(DbRefFactory.UserRef(user.Id)));
                            if (await associatedChat.FirstOrDefaultAsync() is ChatEntity chatFound)
                            {

                                var messages_Unfiltered = await _dbContext.GetMessageCollection().FetchDBRefAsAsync(chatFound.Messages.Reverse().Skip(chat.MessageOffset).Take(AmountOfMessagesAtOnce).ToList());
                                IList<MessageResponseDto> messages = new List<MessageResponseDto>();
                                foreach (MessageEntity messageEntity in messages_Unfiltered)
                                {
                                    var sender_entity_unsafe = await _dbContext.GetUserCollection().FetchDBRefAsAsync(messageEntity.Sender);
                                    var sender = new UserResponseDto
                                    {
                                        Username = sender_entity_unsafe.Username,
                                        FirstName = sender_entity_unsafe.FirstName,
                                        LastName = sender_entity_unsafe.LastName
                                    };
                                    messages.Add(new MessageResponseDto
                                    {
                                        Sender = sender,
                                        Time = messageEntity.Time,
                                        Message = messageEntity.Message
                                    });
                                }
                                ChatMessagesResponseDto chatReturn = new ChatMessagesResponseDto
                                {
                                    ChatId = chat.ChatId,
                                    Type = chat.TypeChat,
                                    Messages = messages
                                };
                                return chatReturn.ToString();
                            }
                            else
                            {
                                // if we want to chat w' someone that isn't on our chat history.
                                ChatMessagesResponseDto chatReturn = new ChatMessagesResponseDto
                                {
                                    ChatId = chat.ChatId,
                                    Type = chat.TypeChat,
                                    Messages = new List<MessageResponseDto>()
                                };
                                return chatReturn.ToString();
                            }
                        }
                        else
                        {
                            return new ResponseDto(21, "Chat error", "Couldn't find user.").ToString();
                        }
                    }
                    else if (chat.TypeChat == ChatType.Group)
                    {
                        try
                        {
                            //ulong GroupId = Convert.ToUInt64(chat.ChatId);
                            var associatedChat = await _dbContext.GetChatCollection().FindAsync(c => c.ChatId == chat.ChatId/*GroupId*/ &&
                                                                          c.Participants.Contains(DbRefFactory.UserRef(user.Id)));
                            if (await associatedChat.FirstOrDefaultAsync() is ChatEntity chatFound)
                            {
                                var messages_Unfiltered = await _dbContext.GetMessageCollection().FetchDBRefAsAsync(chatFound.Messages.Reverse().Skip(chat.MessageOffset).Take(AmountOfMessagesAtOnce).ToList());
                                IList<MessageResponseDto> messages = new List<MessageResponseDto>();
                                foreach (MessageEntity messageEntity in messages_Unfiltered)
                                {
                                    var sender_entity_unsafe = await _dbContext.GetUserCollection().FetchDBRefAsAsync(messageEntity.Sender);
                                    var sender = new UserResponseDto
                                    {
                                        Username = sender_entity_unsafe.Username,
                                        FirstName = sender_entity_unsafe.FirstName,
                                        LastName = sender_entity_unsafe.LastName
                                    };
                                    messages.Add(new MessageResponseDto
                                    {
                                        Sender = sender,
                                        Time = messageEntity.Time,
                                        Message = messageEntity.Message
                                    });
                                }
                                ChatMessagesResponseDto chatReturn = new ChatMessagesResponseDto
                                {
                                    ChatId = chat.ChatId,
                                    Type = chat.TypeChat,
                                    Messages = messages
                                };
                                return chatReturn.ToString();
                            }
                            else
                            {
                                return new ResponseDto(21, "Chat error", "Couldn't find group chat.").ToString();
                            }
                        }
                        catch (OverflowException)
                        {
                            return new ResponseDto(22, "Id error", "Provided invalid group Id.").ToString();
                        }
                        catch (FormatException)
                        {
                            return new ResponseDto(23, "Id error", "Provided invalid group Id.").ToString();
                        }
                        catch (Exception)
                        {
                            return new ResponseDto(0, "Unknown error", "Error occured on get messages.").ToString();
                        }
                    }
                    else
                    {
                        return new ResponseDto(21, "Chat error", "Invalid chat type.").ToString();
                    }
                }
                else
                {
                    return new ResponseDto(23, "Id error", "Provided null Id.").ToString();
                }
            }
            else
            {
                return new ResponseDto(21, "Chat error", "Provided invalid token.").ToString();
            }
        }
    }
}
