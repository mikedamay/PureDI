using System;
using System.Reflection;

namespace PureDI.Tree
{
    internal class Info
    {
        public MemberInfo FieldOrPropertyInfo { get; }
        public ParameterInfo ParameterInfo { get; }

        public Info(MemberInfo fieldOrPropertyInfo)
        {
            this.FieldOrPropertyInfo = fieldOrPropertyInfo;
        }

        public Info(ParameterInfo parameterInfo)
        {
            this.ParameterInfo = parameterInfo;
        }

        public string Name
        {
            get
            {
                if (FieldOrPropertyInfo != null)
                {
                    return FieldOrPropertyInfo.Name;
                }
                else
                {
                    return ParameterInfo.Name;
                }
            }
        }
        public Type Type
        {
            get
            {
                if (FieldOrPropertyInfo != null)
                {
                    return FieldOrPropertyInfo.GetPropertyOrFieldType();
                }
                else
                {
                    return ParameterInfo.ParameterType;
                }
            }
        }

        public bool IsWriteable
        {
            get
            {
                if (FieldOrPropertyInfo != null)
                {
                    return FieldOrPropertyInfo.CanWriteToFieldOrProperty();
                    // for certain properties the bean can
                    // be created but can't be assigned to the member
                }
                else
                {
                    return true;    // for a parameter there is not concept of being writeable
                    // the created bean can always be used in a constructor
                }
            }
        }

        public T GetCustomeAttribute<T>() where T :  Attribute
        {
            if (FieldOrPropertyInfo != null)
            {
                return FieldOrPropertyInfo.GetCustomAttribute<T>();
            }
            else
            {
                return ParameterInfo.GetCustomAttribute<T>();
            }

        }
    }
}