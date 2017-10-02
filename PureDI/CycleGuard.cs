using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PureDI.Common.Common;

namespace PureDI
{
    internal class CycleGuard
    {
        private readonly HashSet<Type> types = new HashSet<Type>();
        private readonly Stack<Type> typeStack = new Stack<Type>();
        public void Push(Type type)
        {
            Assert(!types.Contains(type));
            typeStack.Push(type);
            types.Add(type);
        }
        public Type Pop()
        {
            Type type = typeStack.Pop();
            Assert(types.Contains(type));
            types.Remove(type);
            return type;
        }
        public bool IsPresent(Type type) => types.Contains(type);
    }
}
