using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class IOCObjectTree
    {
        private readonly string profile;
        private readonly IDictionary<(Type, string), Type> typeMap; 

        public IOCObjectTree(string profile
          , IDictionary<(Type type, string name), Type> typeMap)
        {
            this.profile = profile;
            this.typeMap = typeMap;
        }
       // TODO complete the documentation item 3 below if and when factory types are implemented
        /// <summary>
        /// 1. mainly used to create the complete object tree at program startup
        /// 2. may be used to create object tree fragments when running tests
        /// 3. may be used to create an object or link to an existing object
        /// </summary>
        /// <typeparam name="TRootType">The concrete class (not an interface) of the top object in the tree</typeparam>
        /// <returns>an ojbect of root type</returns>
        public TRootType GetOrCreateObjectTree<TRootType>(ref IOCCDiagnostics diagnostics)
        {
            try
            {
                IDictionary<(Type, string), object> mapObjectsCreatedSoFar =
                    new Dictionary<(Type, string), object>();
                object rootObject = Construct(typeof(TRootType));
                mapObjectsCreatedSoFar[(typeof(TRootType), IOCC.DEFAULT_DEPENDENCY_NAME)] = rootObject;
                CreateObjectTree(rootObject, mapObjectsCreatedSoFar, diagnostics);
                if (!(rootObject is TRootType))
                {
                    throw new Exception($"object created by IOC container is not {typeof(TRootType).Name} as expected");
                }
                return (TRootType) rootObject;
            }
            catch (IOCCNoArgConstructorException inace)
            {
                dynamic diagnostic = diagnostics.Groups["MissingNoArgConstructor"].CreateDiagnostic();
                diagnostic.Class = typeof(TRootType).FullName;
                diagnostics.Groups["MissingNoArgConstructor"].Add(diagnostic);
                throw new IOCCException("Failed to create object tree - see diagnostics for details");
            }
        }

        /// <summary>
        /// see documentation for GetOrCreateObjectTree
        /// </summary>
        private object CreateObjectTree(object bean, IDictionary<(Type type, string beanName)
          , object> mapObjectsCreatedSoFar, IOCCDiagnostics diagnostics)
        {
            var fieldOrPropertyInfos = bean.GetType().GetMembers(
              BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
              .Where(f => f is FieldInfo || f is PropertyInfo);
            foreach (var fieldOrPropertyInfo in fieldOrPropertyInfos)
            {
                IOCCInjectedDependencyAttribute attr;
                if ((attr = fieldOrPropertyInfo.GetCustomAttribute<IOCCInjectedDependencyAttribute>()) != null)
                {
                    System.Diagnostics.Debug.Assert(fieldOrPropertyInfo is FieldInfo 
                      || fieldOrPropertyInfo is PropertyInfo);
                    (Type type, string beanName) beanId =
                        (fieldOrPropertyInfo.GetPropertyOrFieldType(), attr.Name);
                    if (typeMap.ContainsKey(beanId))
                    {
                        Type implementationType = typeMap[beanId];
                        object memberBean;
                        if (mapObjectsCreatedSoFar.ContainsKey((implementationType, beanId.beanName))
                          )
                        {
                            memberBean = mapObjectsCreatedSoFar[(implementationType, beanId.beanName)];
                            fieldOrPropertyInfo.SetValue(bean, memberBean);
                        }
                        else if (!fieldOrPropertyInfo.CanWriteToFieldOrProperty(bean))
                        {
                            dynamic diag = diagnostics.Groups["ReadOnlyProperty"].CreateDiagnostic();
                            diag.Class = bean.GetType().FullName;
                            diag.Member = fieldOrPropertyInfo.Name;
                            diagnostics.Groups["ReadOnlyProperty"].Add(diag);
                        }
                        else
                        {
                            try
                            {
                                // reference has not yet been resolved
                                memberBean = Construct(implementationType);
                                mapObjectsCreatedSoFar[(implementationType, beanId.beanName)] = memberBean;
                                fieldOrPropertyInfo.SetValue(bean, memberBean);
                                CreateObjectTree(memberBean, mapObjectsCreatedSoFar, diagnostics);                           
                            }
                            catch (IOCCNoArgConstructorException inace)
                            {
                                dynamic diagnostic = diagnostics.Groups["MissingNoArgConstructor"].CreateDiagnostic();
                                diagnostic.Class = inace.Class;
                                diagnostics.Groups["MissingNoArgConstructor"].Add(diagnostic);
                            }
                        }
                    }

                }
            }
            return bean;
        }
        /// <summary>checks if the type to be instantiated has an empty constructor and if so constructs it</summary>
        /// <param name="beanType">a concrete clasws typically part of the object tree being instantiated</param>
        /// <exception>InvalidArgumentException</exception>  
        private object Construct(Type beanType)
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var constructorInfos = beanType.GetConstructors(flags);
            var noArgConstructorInfo = constructorInfos.FirstOrDefault(ci => ci.GetParameters().Length == 0);
            if (noArgConstructorInfo == null)
            {
                throw new IOCCNoArgConstructorException(beanType.FullName);
            }
            return noArgConstructorInfo.Invoke(flags | BindingFlags.CreateInstance, null, new object[0], null);

        }
    }

    internal class IOCCNoArgConstructorException : Exception
    {
        public string Class { get; }
        public IOCCNoArgConstructorException(string _class)
        {
            Class = _class;
        }
    }

    internal static class IOCCExtensions
    {
        public static Type GetPropertyOrFieldType(this MemberInfo memberInfo)
        {
            System.Diagnostics.Debug.Assert( memberInfo is FieldInfo || memberInfo is PropertyInfo);
            return (memberInfo as FieldInfo)?.FieldType ?? (memberInfo as PropertyInfo).PropertyType;
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

        public static bool CanWriteToFieldOrProperty(this MemberInfo memberInfo, object bean)
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
    }
}
