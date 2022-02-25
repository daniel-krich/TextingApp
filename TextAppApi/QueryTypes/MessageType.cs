using HotChocolate.Types;
using HotChocolate.Types.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppApi.QueryResolvers;
using TextAppData.DataEntities;

namespace TextAppApi.QueryTypes
{
    public class MessageType : ObjectType<MessageEntity>
    {
        protected override void Configure(IObjectTypeDescriptor<MessageEntity> descriptor)
        {
            descriptor.Field(_ => _.Id).Type<ObjectIdType>();
            descriptor.Field(_ => _.Message).Type<StringType>();
            descriptor.Field(_ => _.Time).Type<DateTimeType>();


            descriptor.Field(_ => _.Sender).ResolveWith<ChatResolver>(o => o.GetSender(default, default));
        }
    }
}
