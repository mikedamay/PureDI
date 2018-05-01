using System;

namespace PureDI.Tree
{
    internal class InstantiatedBeanId
    {
        private readonly Type _type;
        private readonly string _beanName;
        private readonly string _constructorName;
        
        public InstantiatedBeanId(Type type, string beanName, string constructorName)
        {
            _type = type;
            _beanName = beanName;
            _constructorName = constructorName;
        }
        protected bool Equals(InstantiatedBeanId other)
        {
            return Equals(_type, other._type) && string.Equals(_beanName, other._beanName) && string.Equals(_constructorName, other._constructorName);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((InstantiatedBeanId) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (_type != null ? _type.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_beanName != null ? _beanName.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_constructorName != null ? _constructorName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
    
}