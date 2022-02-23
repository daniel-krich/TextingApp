using HotChocolate.Types;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextAppApi.QueryTypes
{
    public class DBRefType : ObjectType<MongoDBRef>
    {
        protected override void Configure(IObjectTypeDescriptor<MongoDBRef> descriptor)
        {
            descriptor.Field(_ => _.Id).Type<IdType>();
            descriptor.Field(_ => _.CollectionName).Type<StringType>();
            descriptor.Field(_ => _.DatabaseName).Type<StringType>();
        }
    }
}
