using System;
using System.Linq;
using System.Reflection;
using PureDI.Attributes;
using PureDI.Common;

namespace PureDI.Tree
{
    internal static class TreeExtensions
    {
        public static Type GetPropertyOrFieldType(this MemberInfo memberInfo)
        {
            Common.Common.Assert(memberInfo is FieldInfo || memberInfo is PropertyInfo);
            return (memberInfo as FieldInfo)?.FieldType ?? (memberInfo as PropertyInfo).PropertyType;
        }
        public static bool IsPropertyOrField(this MemberInfo memberInfo)
        {
            return memberInfo is FieldInfo || memberInfo is PropertyInfo;
        }

        public static void SetValue(this MemberInfo memberInfo, object bean, object memberBean)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    field.SetValue(bean, memberBean);
                    break;
                case PropertyInfo property:
                    property.SetValue(bean, memberBean);
                    break;
                default:
                    throw new IOCCInternalException(
                        $"GetValue extension method encountered a MemberInfo instances that was not a field or property: {memberInfo.GetType()}"
                        , null);
            }
        }

        public static object GetValue(this MemberInfo memberInfo, object bean)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    return field.GetValue(bean);
                case PropertyInfo property:
                    return property.GetValue(bean);
                default:
                    throw new IOCCInternalException(
                        $"GetValue extension method encountered a MemberInfo instances that was not a field or property: {memberInfo.GetType()}"
                        , null);
            }
        }
        public static Type GetDeclaredType(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    return field.FieldType;
                case PropertyInfo property:
                    return property.PropertyType;
                default:
                    throw new IOCCInternalException(
                        $"GetDeclaredType extension method encountered a MemberInfo instances that was not a field or property: {memberInfo.GetType()}"
                        , null);
            }
        }

        public static bool CanWriteToFieldOrProperty(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    return true;        // are there fields capable of 
                // being assigned an class instance with a no-arg constructorimmune to writing?
                case PropertyInfo property:
                    return property.CanWrite;
                default:
                    throw new IOCCInternalException(
                        $"CanWrite extension method encountered a MemberInfo instances that was not a field or property: {memberInfo.GetType()}"
                        , null);

            }
        }
        public static bool CanReadFromFieldOrProperty(this MemberInfo memberInfo)
        {
            switch (memberInfo)
            {
                case FieldInfo field:
                    return true;
                case PropertyInfo property:
                    return property.CanRead;
                default:
                    throw new IOCCInternalException(
                        $"CanWrite extension method encountered a MemberInfo instances that was not a field or property: {memberInfo.GetType()}"
                        , null);

            }
        }

        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive;
        }

        public static bool HasInjectedConstructorParameters(this Type type, string constructorName)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Any(c => CustomAttributeExtensions.GetCustomAttributes((MemberInfo) c).Any(
                ca => ca is ConstructorBaseAttribute
                      && (ca as ConstructorBaseAttribute).Name?.ToLower() == constructorName));
        }

        public static string GetConstructorNameFromMember(this Type type)
        {
            BeanReferenceBaseAttribute attr = type.GetCustomAttribute<BeanReferenceBaseAttribute>();
            if (attr != null)
            {
                return attr.ConstructorName;
            }
            return Constants.DefaultConstructorName;
        }

        public static ConstructorInfo GetConstructorNamed(this Type type, string name)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(co => co.GetCustomAttribute<
                                          ConstructorBaseAttribute>() != null
                                      && string.Compare(co.GetCustomAttribute<
                                          ConstructorBaseAttribute>().Name, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public static ConstructorInfo GetNoArgConstructor(this Type beanType, BindingFlags flags)
            => beanType.GetConstructors(flags).FirstOrDefault(ci => ci.GetParameters().Length == 0);
    }
}