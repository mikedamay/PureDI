using System;
namespace PureDI.Tree
{
    internal class BeanSpec : InstantiatedBeanId
    {        
        public BeanSpec((Type type, string beanName, string constructorName) beanId)
            : base(beanId.type, beanId.beanName, beanId.constructorName)
        {
        }

        public Type Type => _type;

        public string BeanName => _beanName;

        public string ConstructorName => _constructorName;
    }
}