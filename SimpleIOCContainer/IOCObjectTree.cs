using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace com.TheDisappointedProgrammer.IOCC
{
    internal class IOCObjectTree
    {
        private readonly string profile;
        private readonly IDictionary<(Type type, string beanName), Type> typeMap; 
                // from the point of view of generics the key.type may contain a generic type definition
                // and the value may be a constructed generic type

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
        /// <param name="rootType">The top node in the tree</param>
        /// <returns>an ojbect of root type</returns>
        public object GetOrCreateObjectTree(Type rootType
          ,ref IOCCDiagnostics diagnostics, string rootBeanName)
        {
            try
            {
                System.Diagnostics.Debug.Assert(rootType != null);
                System.Diagnostics.Debug.Assert(rootBeanName != null);
                // the key in the objects created so far map comprises 2 types.  The first is the
                // intended concrete type that will be instantiated.  This works well for
                // non-generic types but for generics the concrete type, which is taken from the typeMap,
                // is a generic type definition.  The builder needs to lay its hands on the type argument
                // to substitute for the generic parameter.  The second type (beanReferenceType) which
                // has been taken from the member information of the declaring task provides the generic argument
                IDictionary<(Type beanType, Type beanReferenceType), object> mapObjectsCreatedSoFar =
                    new Dictionary<(Type, Type), object>();
                var rootObject = CreateObjectTree((rootType, rootBeanName)
                    ,mapObjectsCreatedSoFar, diagnostics, new BeanReferenceDetails());
                if (rootObject != null && !rootType.IsInstanceOfType(rootObject))
                {
                    throw new IOCCInternalException($"object created by IOC container is not {rootType.Name} as expected");
                }
                System.Diagnostics.Debug.Assert(rootObject == null 
                  || rootType.IsInstanceOfType(rootObject));
                return rootObject;
            }
            catch (IOCCNoArgConstructorException inace)
            {
                dynamic diagnostic = diagnostics.Groups["MissingNoArgConstructor"].CreateDiagnostic();
                diagnostic.Class = rootType.GetIOCCName();
                diagnostics.Groups["MissingNoArgConstructor"].Add(diagnostic);
                throw new IOCCException("Failed to create object tree - see diagnostics for details", inace, diagnostics);
            }
        }

        /// <summary>
        /// see documentation for GetOrCreateObjectTree
        /// </summary>
        /// <param name="beanId">the type + beanName for which a bean is to be created.
        ///     The bean will not necessarily have the type passed in as this
        ///     may be a base class or interface (constructed generic type)
        ///     from which the bean is derived</param>
        /// <param name="mapObjectsCreatedSoFar">for all beans instantiated to this point
        ///     maps the name of the class or struct of
        ///     the object to the instance of the object.</param>
        /// <param name="diagnostics"></param>
        /// <param name="beanReferenceDetails">provides a context to the bean that
        ///     can be displayed in diagnostic messages - currently not used for
        ///     anything else</param>
        /// <param name="bean">a class already instantiated by IOCC whose
        ///                    fields and properties may need to be injuected</param>
        private object CreateObjectTree((Type beanType, string beanName) beanId
          , IDictionary<(Type type, Type beanReferenceType), object> mapObjectsCreatedSoFar
          , IOCCDiagnostics diagnostics
          , BeanReferenceDetails beanReferenceDetails)
        {
            object bean;
            try
            {
                Type implementationType;
                if (IsBeanPresntInTypeMap(beanId))
                {
                    implementationType = GetImplementationFromTypeMap(beanId);
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
                        diagnostics.Groups["MissingBean"].Add(diag);
                        return null;
                    }
                }
                if (mapObjectsCreatedSoFar.ContainsKey((implementationType, beanId.beanType)))
                {       // there is a cyclical dependency
                    bean = mapObjectsCreatedSoFar[(implementationType, beanId.beanType)];
                    return bean;
                }
                else
                {
                    Type constructableType = implementationType.IsGenericType
                      ? beanId.beanType : implementationType;
                        // TODO explain why type to be constructed is complicated by generics
                    bean = Construct(constructableType);
                    mapObjectsCreatedSoFar[(implementationType, beanId.beanType)] = bean;
                                           
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
                IOCCBeanReferenceAttribute attr;
                if ((attr = fieldOrPropertyInfo.GetCustomAttribute<IOCCBeanReferenceAttribute>()) != null)
                {
                    System.Diagnostics.Debug.Assert(fieldOrPropertyInfo is FieldInfo
                                                    || fieldOrPropertyInfo is PropertyInfo);
                   (Type type, string beanName) memberBeanId =
                        (fieldOrPropertyInfo.GetPropertyOrFieldType(), attr.Name);
                         object memberBean;
                    if (!fieldOrPropertyInfo.CanWriteToFieldOrProperty(bean))
                    {
                        dynamic diag = diagnostics.Groups["ReadOnlyProperty"].CreateDiagnostic();
                        diag.Class = bean.GetType().GetIOCCName();
                        diag.Member = fieldOrPropertyInfo.Name;
                        diagnostics.Groups["ReadOnlyProperty"].Add(diag);
                    }
                    else    // member is writable
                    {
                        if (attr.Factory != null)
                        {
                            object o =
                              CreateObjectTree((attr.Factory, attr.Name), mapObjectsCreatedSoFar
                              , diagnostics
                              , new BeanReferenceDetails(bean.GetType()
                              , fieldOrPropertyInfo.Name, memberBeanId.beanName));
                            if (o == null)
                            {
                                dynamic diag = diagnostics.Groups["MissingFactory"].CreateDiagnostic();
                                diag.DeclaringBean = bean.GetType().FullName;
                                diag.Member = fieldOrPropertyInfo.Name;
                                diag.Factory = attr.Factory.FullName;
                                diagnostics.Groups["MissingFactory"].Add(diag);
                                memberBean = null;
                            }
                            else
                            {
                                System.Diagnostics.Debug.Assert(o is IOCCFactory);
                                IOCCFactory factory = o as IOCCFactory;
                                memberBean = factory.Execute(new BeanFactoryArgs(
                                  attr.FactoryParameter));
                            }
                        }
                        else
                        {
                            memberBean = CreateObjectTree(memberBeanId, mapObjectsCreatedSoFar
                                , diagnostics
                                , new BeanReferenceDetails(bean.GetType()
                                    , fieldOrPropertyInfo.Name, memberBeanId.beanName));
                        }
                        fieldOrPropertyInfo.SetValue(bean, memberBean);
                    }
                }
            }
            return bean;
        }

        /// <param name="beanid">Typically this is the type ofa member 
        ///     marked as a bean reference with [IOCCBeanReference]
        ///     for generics bean type is a generic type definition</param>
        /// <returns>This will be a concrete class marked as a bean with [IOCCBean] which
        ///     is derived from the beanId.beanType.  For generics this will be a
        ///     constructed generic type</returns>
        private Type GetImplementationFromTypeMap((Type beanType, string beanName) beanId)
        {
            
            char[] a = beanId.beanType.FullName.TakeWhile(n => n != '[').ToArray();
            string beanTypeName =  new String(a);
                    // trim the generic arguments from a generic
            Type referenceType = typeMap.Keys.FirstOrDefault(
              k => k.type.FullName == beanTypeName && k.beanName == beanId.beanName).type;
            return typeMap[(referenceType, beanId.beanName)];

        }

        /// <param name="beanid"><see cref="GetImplementationFromTypeMap"/></param>
        private bool IsBeanPresntInTypeMap((Type beanType, string beanName) beanId)
        {
            char[] a = beanId.beanType.FullName.TakeWhile(n => n != '[').ToArray();
            string beanTypeName =  new String(a);
                    // trim the generic arguments from a generic
            return typeMap.Keys.Any(k => k.type.FullName == beanTypeName && k.beanName == beanId.beanName);
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
