using HotChocolate.Types;
using HotChocolate.Types.MongoDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TextAppData.DataEntities;

namespace TextAppApi.QueryTypes
{
    public class UserType : ObjectType<UserEntity>
    {
        protected override void Configure(IObjectTypeDescriptor<UserEntity> descriptor)
        {
            descriptor.Field(_ => _.Id).Type<ObjectIdType>();
            descriptor.Field(_ => _.Username).Type<StringType>();
            descriptor.Field(_ => _.FirstName).Type<StringType>();
            descriptor.Field(_ => _.LastName).Type<StringType>();
            descriptor.Field(_ => _.Email).Type<StringType>();
            descriptor.Field(_ => _.Password).Type<StringType>().Ignore();
        }
    }
}
