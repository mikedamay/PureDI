using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Security.Policy;
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
        public TRootType GetOrCreateObjectTree<TRootType>(ref IOCCDiagnostics diagnostics, string rootBeanName)
        {
            try
            {
                IDictionary<(Type, string), object> mapObjectsCreatedSoFar =
                    new Dictionary<(Type, string), object>();
                object rootObject;
                rootObject = CreateObjectTree((typeof(TRootType), rootBeanName)
                  ,mapObjectsCreatedSoFar, diagnostics, new BeanReferenceDetails());
                if (!(rootObject is TRootType) && rootObject != null)
                {
                    throw new IOCCInternalException($"object created by IOC container is not {typeof(TRootType).Name} as expected");
                }
                return (TRootType) rootObject;
            }
            catch (IOCCNoArgConstructorException inace)
            {
                dynamic diagnostic = diagnostics.Groups["MissingNoArgConstructor"].CreateDiagnostic();
                diagnostic.Class = typeof(TRootType).GetIOCCName();
                diagnostics.Groups["MissingNoArgConstructor"].Add(diagnostic);
                throw new IOCCException("Failed to create object tree - see diagnostics for details", inace, diagnostics);
            }
        }

        /// <summary>
        /// see documentation for GetOrCreateObjectTree
        /// </summary>
        /// <param name="beanId"></param>
        /// <param name="mapObjectsCreatedSoFar">for all beans instantiated to this point
        ///     maps the name of the class or struct of
        ///     the object to the instance of the object.</param>
        /// <param name="diagnostics"></param>
        /// <param name="beanReferenceDetails"></param>
        /// <param name="bean">a class already instantiated by IOCC whose
        ///                    fields and properties may need to be injuected</param>
        private object CreateObjectTree((Type beanType, string beanName) beanId
          , IDictionary<(Type type, string beanName), object> mapObjectsCreatedSoFar
          , IOCCDiagnostics diagnostics
          , BeanReferenceDetails beanReferenceDetails)
        {
            object bean;
            try
            {
                Type implementationType = null;
                if (typeMap.ContainsKey(beanId))
                {
                    implementationType = typeMap[beanId];
                }
                else
                {
                    if (beanReferenceDetails.IsRoot)
                    {
                        dynamic diag = diagnostics.Groups["MissingRoot"].CreateDiagnostic();
                        diag.BeanType = beanId.beanType.GetIOCCName();
                        diag.BeanName = beanId.beanName;
                        diagnostics.Groups["MissingRoot"].Add(diag);
                        throw new IOCCException("failed to create object tree - see diagnostics for detail",
                            diagnostics);
                    }
                    else
                    {
                        dynamic diag = diagnostics.Groups["MissingBean"].CreateDiagnostic();
                        diag.Bean = beanReferenceDetails.DeclaringType.GetIOCCName();
                        diag.MemberType = beanId.beanType.GetIOCCName();
                        diag.MemberName = beanReferenceDetails.MemberName;
                        diag.MemberBeanName = beanReferenceDetails.MemberBeanName;
                        //(diag.Bean, diag.MemberName, diag.MemberBeanName) = beanReferenceDetails;
                        diagnostics.Groups["MissingBean"].Add(diag);
                        return null;
                    }
                }
                if (mapObjectsCreatedSoFar.ContainsKey((implementationType, beanId.beanName)))
                {       // there is a cyclical dependency
                    bean = mapObjectsCreatedSoFar[(implementationType, beanId.beanName)];
                    return bean;
                }
                else
                {
                    bean = Construct(implementationType);
                    mapObjectsCreatedSoFar[(implementationType, beanId.beanName)] = bean;
                   
                }
            }
            catch (IOCCNoArgConstructorException inace)
            {
                dynamic diagnostic = diagnostics.Groups["MissingNoArgConstructor"].CreateDiagnostic();
                diagnostic.Class = inace.Class;
                diagnostics.Groups["MissingNoArgConstructor"].Add(diagnostic);
                return null;
            }
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
                   (Type type, string beanName) memberBeanId =
                        (fieldOrPropertyInfo.GetPropertyOrFieldType(), attr.Name);
                    //if (typeMap.ContainsKey(memberBeanId))
                    //{
                        object memberBean;
                        //Type memberImplementationType = typeMap[memberBeanId];
                        //if (mapObjectsCreatedSoFar.ContainsKey((memberImplementationType, memberBeanId.beanName))
                        //)
                        //{
                        //    memberBean = mapObjectsCreatedSoFar[(memberImplementationType, memberBeanId.beanName)];
                        //    fieldOrPropertyInfo.SetValue(bean, memberBean);
                        //}
                        /*else */ if (!fieldOrPropertyInfo.CanWriteToFieldOrProperty(bean))
                        {
                            dynamic diag = diagnostics.Groups["ReadOnlyProperty"].CreateDiagnostic();
                            diag.Class = bean.GetType().GetIOCCName();
                            diag.Member = fieldOrPropertyInfo.Name;
                            diagnostics.Groups["ReadOnlyProperty"].Add(diag);
                        }
                        else
                        {
                            
                            memberBean = CreateObjectTree(memberBeanId, mapObjectsCreatedSoFar
                              , diagnostics
                              , new BeanReferenceDetails(bean.GetType()
                              , fieldOrPropertyInfo.Name, memberBeanId.beanName));
                            fieldOrPropertyInfo.SetValue(bean, memberBean);
                        }
                    //}
                    //else
                    //{
                    //    dynamic diag = diagnostics.Groups["MissingBean"].CreateDiagnostic();
                    //    diag.Bean = bean.GetType().GetIOCCName();
                    //    diag.MemberType = memberBeanId.type.GetIOCCName();
                    //    diag.MemberName = fieldOrPropertyInfo.Name;
                    //    diag.MemberBeanName = memberBeanId.beanName;
                    //    diagnostics.Groups["MissingBean"].Add(diag);
                    //}
                }
            }
            return bean;
        }
        /// <summary>checks if the type to be instantiated has an empty constructor and if so constructs it</summary>
        /// <param name="beanType">a concrete clasws typically part of the object tree being instantiated</param>
        /// <exception>InvalidArgumentException</exception>  
        private object Construct(Type beanType)
        {
            if (beanType.IsStruct())
            {
                return Activator.CreateInstance(beanType);
            }
            else
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                var constructorInfos = beanType.GetConstructors(flags);
                var noArgConstructorInfo = constructorInfos.FirstOrDefault(ci => ci.GetParameters().Length == 0);
                if (noArgConstructorInfo == null)
                {
                    throw new IOCCNoArgConstructorException(beanType.GetIOCCName());
                }
                return noArgConstructorInfo.Invoke(flags | BindingFlags.CreateInstance, null, new object[0], null);
                
            }

        }

        private class BeanReferenceDetails
        {
            private Type declaringType;
            private string memberName;
            private string memberBeanName;
            public bool IsRoot { get; }

            public Type DeclaringType
              => IsRoot
              ? throw new IOCCInternalException("there are no reference details for the root bean of the tree")
              : declaringType;
            public string MemberName
              => IsRoot
              ? throw new IOCCInternalException("there are no reference details for the root bean of the tree")
              : memberName;
            public string MemberBeanName
              => IsRoot
              ? throw new IOCCInternalException("there are no reference details for the root bean of the tree")
              : memberBeanName;

        
            public BeanReferenceDetails()
            {
                this.IsRoot = true;
            }

            public BeanReferenceDetails(Type declaringType, string memberName, string memberBeanName)
            {
                this.declaringType = declaringType;
                this.memberName = memberName;
                this.memberBeanName = memberBeanName;
            }

            public void Deconstruct(out Type DeclaringType, out string MemberName, out string MemberBeanName)
            {
                if (this.IsRoot)
                {
                    throw new IOCCInternalException("there are no reference details for the root bean of the tree");
                }
                DeclaringType = declaringType;
                MemberName = memberName;
                MemberBeanName = memberBeanName;
            }
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

    internal static class IOCCTreeExtensions
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

        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive;
        }
    }
}
