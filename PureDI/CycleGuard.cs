using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PureDI.Tree;
using static PureDI.Common.Common;

namespace PureDI
{
    internal class CycleGuard
    {
        private readonly HashSet<ConstructableBean> _beans = new HashSet<ConstructableBean>();
        private readonly Stack<ConstructableBean> _beanStack = new Stack<ConstructableBean>();
        public void Push(ConstructableBean constructableBean)
        {
            Assert(!_beans.Contains(constructableBean));
            _beanStack.Push(constructableBean);
            _beans.Add(constructableBean);
        }
        public ConstructableBean Pop()
        {
            ConstructableBean constructableBean = _beanStack.Pop();
            Assert(_beans.Contains(constructableBean));
            _beans.Remove(constructableBean);
            return constructableBean;
        }
        public bool IsPresent(ConstructableBean constructableBean) => _beans.Contains(constructableBean);
    }
}
