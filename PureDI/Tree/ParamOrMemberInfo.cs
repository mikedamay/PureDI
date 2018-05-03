using System;
using System.Reflection;
using PureDI.Attributes;
using static PureDI.Common.Common;

namespace PureDI.Tree
{
    internal class ParamOrMemberInfo
    {
        public MemberInfo FieldOrPropertyInfo { get; }
        public ParameterInfo ParameterInfo { get; }

        public ParamOrMemberInfo(MemberInfo fieldOrPropertyInfo)
        {
            this.FieldOrPropertyInfo = fieldOrPropertyInfo;
        }

        public ParamOrMemberInfo(ParameterInfo parameterInfo)
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

        public bool IsFactory => GetBeanReferenceAttribute().Factory != null;
        
        public BeanReferenceBaseAttribute GetBeanReferenceAttribute()
        {
            BeanReferenceBaseAttribute attr;
            attr = this.GetCustomeAttribute<BeanReferenceBaseAttribute>();
            return attr;
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
        public static implicit operator ParameterInfo(ParamOrMemberInfo p)
        {
            Assert(p.ParameterInfo != null);
            return p.ParameterInfo;
        }
        public static implicit operator MemberInfo(ParamOrMemberInfo p)
        {
            Assert(p.FieldOrPropertyInfo != null);
            return p.FieldOrPropertyInfo;
        }
    }
}