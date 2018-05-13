using System;
using PureDI.Common;

namespace PureDI.Tree
{
    internal class ConstructableBean : InstantiatedBeanId
    {
        public ConstructableBean(Type type, string beanName) 
          : base(type, beanName, Constants.DefaultConstructorName)
        {
        }

        public Type Type => _type;
        public string BeanName => _beanName;
        
    }
}