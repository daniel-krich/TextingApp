using HotChocolate.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TextAppApi.QueryResolvers
{
    public static class FilterResolverExtention
    {
        private const string Take = "take";
        private const string Skip = "skip";
        public static IEnumerable<T> FilterByContext<T>(this IEnumerable<T> collection, IResolverContext context)
        {
            var skipParam = context.ArgumentValue<int>(Skip);
            var takeParam = context.ArgumentValue<int>(Take);
            return collection.Skip(skipParam).Take(takeParam == 0 ? int.MaxValue : takeParam);
        }
    }
}
