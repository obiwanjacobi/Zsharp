using Mono.Cecil;
using Mono.Collections.Generic;
using System.Linq;

namespace Zsharp.Emit
{
    public static class CecilExtensions
    {
        public static T? Find<T>(this Collection<T> collection, string name)
            where T : class, IMemberDefinition
        {
            return collection?.SingleOrDefault(i => i.Name == name);
        }
    }
}
