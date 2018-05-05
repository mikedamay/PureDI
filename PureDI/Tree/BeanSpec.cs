using System;
namespace PureDI.Tree
{
    internal class BeanSpec : InstantiatedBeanId
    {        
        public BeanSpec(Type type, string beanName, string constructorName)
            : base(type, beanName, constructorName)
        {
        }

        public Type Type => _type;

        public string BeanName => _beanName;

        public string ConstructorName => _constructorName;
    }
}