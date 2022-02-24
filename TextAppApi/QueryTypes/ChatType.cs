using HotChocolate;
using HotChocolate.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TextAppApi.QueryResolvers;
using TextAppData.DataContext;
using TextAppData.DataEntities;
using HotChocolate.Types.MongoDb;

namespace TextAppApi.QueryTypes
{
    public class ChatType : ObjectType<ChatEntity>
    {
        protected override void Configure(IObjectTypeDescriptor<ChatEntity> descriptor)
        {
            descriptor.Field(_ => _.Id).Type<ObjectIdType>();
            descriptor.Field(_ => _.GroupId).Type<UnsignedLongType>();
            descriptor.Field(_ => _.Name).Type<StringType>();
            descriptor.Field(_ => _.Type).Type<EnumType<TextAppData.Enums.ChatType>>();

            descriptor.Field<ParticipantsResolver>(o => o.GetParticipantsCount(default));
            descriptor.Field<MessagesResolver>(o => o.GetMessagesCount(default));

            descriptor.Field<MessagesResolver>(o => o.GetLastMessage(default, default));

            descriptor.Field(_ => _.Participants).Type<ListType<UserType>>().ResolveWith<ParticipantsResolver>(o => o.GetParticipants(default, default));
            descriptor.Field(_ => _.Messages).Type<ListType<MessageType>>().ResolveWith<MessagesResolver>(o => o.GetMessages(default, default));
        }
    }
}
