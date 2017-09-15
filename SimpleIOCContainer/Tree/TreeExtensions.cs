using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace com.TheDisappointedProgrammer.IOCC.Tree
{
    internal static class TreeExtensions
    {
        public static Type GetPropertyOrFieldType(this MemberInfo memberInfo)
        {
            Common.Assert(memberInfo is FieldInfo || memberInfo is PropertyInfo);
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

        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive;
        }

        public static bool HasInjectedConstructorParameters(this Type type, string constructorName)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Any(c => CustomAttributeExtensions.GetCustomAttributes((MemberInfo) c).Any(
                ca => ca is ConstructorBaseAttribute
                      && (ca as ConstructorBaseAttribute).Name == constructorName));
        }

        public static string GetConstructorNameFromMember(this Type type)
        {
            BeanReferenceBaseAttribute attr = type.GetCustomAttribute<BeanReferenceBaseAttribute>();
            if (attr != null)
            {
                return attr.ConstructorName;
            }
            return SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME;
        }

        public static ConstructorInfo GetConstructorNamed(this Type type, string name)
        {
            return type.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .FirstOrDefault(co => co.GetCustomAttribute<
                                          ConstructorBaseAttribute>() != null
                                      && string.Compare(co.GetCustomAttribute<
                                          ConstructorBaseAttribute>().Name, name, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}