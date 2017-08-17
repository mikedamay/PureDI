using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static com.TheDisappointedProgrammer.IOCC.Common;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class CycleGuard
    {
        private HashSet<Type> types = new HashSet<Type>();
        private Stack<Type> typeStack = new Stack<Type>();
        private HashSet<Type> cyclicalDependencies = new HashSet<Type>();
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
        // bye bye single responsibility!
        public void AddCyclicalDependency(Type type) => cyclicalDependencies.Add(type);
        public bool IsCyclicalDependency(Type type) => cyclicalDependencies.Contains(type);
        public void RemoveCyclicalDependency(Type type) => cyclicalDependencies.Remove(type);
    }
    //internal class CycleGuard
    //{
    //    private Stack<HashSet<(Type beanType, string beanName)>> stack 
    //      = new Stack<HashSet<(Type beanType, string beanName)>>();
    //    public void PushFrame()
    //    {
    //        stack.Push(new HashSet<(Type beanType, string beanName)>());
    //    }
    //    public void PopFrame() => stack.Pop();
    //    public void Add((Type, string) spec)
    //    {
    //        stack.Peek().Add(spec);
    //    }
    //    public bool IsPresent((Type beanType, string beanName) beanId)
    //    {
    //        return stack.Any(f => f.Contains(beanId));
    //    }
    //}
}
