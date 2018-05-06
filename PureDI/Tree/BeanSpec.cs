using System;
namespace PureDI.Tree
{
    internal class BeanSpec : InstantiatedBeanId
    {
        public class TypeMapKey
        {
            private readonly BeanSpec _beanSpec;
            public TypeMapKey(BeanSpec beanSpec)
            {
                _beanSpec = beanSpec;
            }
            protected bool Equals(TypeMapKey other)
            {
                return Equals(_beanSpec._type, other._beanSpec._type) 
                  && string.Equals(_beanSpec._beanName, other._beanSpec._beanName);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((TypeMapKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _beanSpec?._type?.GetHashCode() ?? 0;
                    hashCode = (hashCode * 397) ^ _beanSpec?._beanName?.GetHashCode() ?? 0;
                    return hashCode;
                }
            }
        }
        public BeanSpec(Type type, string beanName, string constructorName)
            : base(type, beanName, constructorName)
        {
            _typeMapKey = new TypeMapKey(this);
        }

        private readonly TypeMapKey _typeMapKey;
        
        public static implicit operator TypeMapKey(BeanSpec bs)
        {
            return bs._typeMapKey;
        }
        public Type Type => _type;

        public string BeanName => _beanName;

        public string ConstructorName => _constructorName;
    }
}