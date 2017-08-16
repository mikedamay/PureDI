using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using static com.TheDisappointedProgrammer.IOCC.Common;

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
        /// <param name="diagnostics"></param>
        /// <param name="rootBeanName"></param>
        /// <param name="scope"></param>
        /// <param name="mapObjectsCreatedSoFar"></param>
        /// <param name="mapObjectsCreatedSoFar1"></param>
        /// <returns>an ojbect of root type</returns>
        public object GetOrCreateObjectTree(Type rootType
          , ref IOCCDiagnostics diagnostics
          , string rootBeanName, BeanScope scope, IDictionary<(Type beanType
          , Type beanReferenceType), object> mapObjectsCreatedSoFar)
        {
            try
            {
                Assert(rootType != null);
                Assert(rootBeanName != null);
                var rootObject = CreateObjectTree((rootType, rootBeanName)
                    ,mapObjectsCreatedSoFar, diagnostics, new BeanReferenceDetails(), scope);
                if (rootObject != null && !rootType.IsInstanceOfType(rootObject))
                {
                    throw new IOCCInternalException($"object created by IOC container is not {rootType.Name} as expected");
                }
                Assert(rootObject == null 
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
        /// <param name="beanScope"></param>
        /// <param name="fieldOrPropertyInfo1">used to determine the scope of the bean to be created</param>
        /// <param name="bean">a class already instantiated by IOCC whose
        ///                    fields and properties may need to be injuected</param>
        private object CreateObjectTree((Type beanType, string beanName) beanId
          , IDictionary<(Type type, Type beanReferenceType), object> mapObjectsCreatedSoFar
          , IOCCDiagnostics diagnostics, BeanReferenceDetails beanReferenceDetails, BeanScope beanScope)
        {
            Type implementationType;
            (bool constructionComplete, object beanId) MakeBean()
            {
                object constructedBean;
                try
                {
                    if (beanScope != BeanScope.Prototype
                      && mapObjectsCreatedSoFar.ContainsKey((implementationType, beanId.beanType)))
                    {
                        // there maybe a cyclical dependency
                        constructedBean = mapObjectsCreatedSoFar[(implementationType, beanId.beanType)];
                        return (true, constructedBean);
                    }
                    else
                    {
                        Type constructableType = implementationType.IsGenericType
                            ? beanId.Item1
                            : implementationType;
                        // TODO explain why type to be constructed is complicated by generics
                        constructedBean = Construct(constructableType);
                        if (beanScope != BeanScope.Prototype)
                        {
                            mapObjectsCreatedSoFar[(implementationType, beanId.beanType)] = constructedBean;
                        }
                    }
                }
                catch (IOCCNoArgConstructorException inace)
                {
                    RecordDiagnostic(diagnostics, "MissingNoArgConstructor"
                      , ("Class", inace.Class));
                    return (true, null);
                }
                return (false, constructedBean);
            }           // MakeBean()
            void CreateChildren(Type declaringBeanType
              , out List<MemberSpec> members)
            {
                members = new List<MemberSpec>();
                var fieldOrPropertyInfos = declaringBeanType.GetMembers(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(f => f is FieldInfo || f is PropertyInfo);
                foreach (var fieldOrPropertyInfo in fieldOrPropertyInfos)
                {
                    IOCCBeanReferenceAttribute attr;
                    if ((attr = fieldOrPropertyInfo.GetCustomAttribute<IOCCBeanReferenceAttribute>()) != null)
                    {
                        Assert(fieldOrPropertyInfo is FieldInfo
                          || fieldOrPropertyInfo is PropertyInfo);
                        (Type type, string beanName) memberBeanId =
                            (fieldOrPropertyInfo.GetPropertyOrFieldType(), attr.Name);
                        object memberBean;
                        if (!fieldOrPropertyInfo.CanWriteToFieldOrProperty())
                        {
                            RecordDiagnostic(diagnostics, "ReadOnlyProperty"
                                , ("Class", declaringBeanType.GetIOCCName())
                                , ("Member", fieldOrPropertyInfo.Name));
                        }
                        else // member is writable
                        {
                            if (attr.Factory != null)
                            {
                                // create the factory
                                object
                                o = CreateObjectTree((attr.Factory, attr.Name), mapObjectsCreatedSoFar
                                  , diagnostics
                                  , new BeanReferenceDetails(declaringBeanType
                                  , fieldOrPropertyInfo.Name, memberBeanId.beanName), attr.Scope);
                                if (o == null)
                                {
                                    RecordDiagnostic(diagnostics, "MissingFactory"
                                        , ("DeclaringBean", declaringBeanType.FullName)
                                        , ("Member", fieldOrPropertyInfo.Name)
                                        , ("Factory", attr.Factory.FullName)
                                        , ("ExpectedType", fieldOrPropertyInfo.MemberType));
                                }
                                else if (!(o is IOCCFactory))
                                {
                                    RecordDiagnostic(diagnostics, "BadFactory"
                                        , ("DeclaringBean", declaringBeanType.FullName)
                                        , ("Member", fieldOrPropertyInfo.Name)
                                        , ("Factory", attr.Factory.FullName)
                                    );
                                }
                                else    // factory successfully created
                                {
                                    IOCCFactory factoryBean = (o as IOCCFactory);
                                    members.Add(new MemberSpec(fieldOrPropertyInfo, factoryBean, true, null));
                                }
                            }
                            else    // create the member without using a factory
                            {
                                memberBean = CreateObjectTree(memberBeanId, mapObjectsCreatedSoFar
                                    , diagnostics
                                    , new BeanReferenceDetails(declaringBeanType
                                        , fieldOrPropertyInfo.Name, memberBeanId.beanName), attr.Scope);
                                members.Add(new MemberSpec(fieldOrPropertyInfo, memberBean, false, null));
                            }       // not a factory
                        }           // writeable member
                    }               // this is a bean reference
                }                   // for each property or field
            }                       // CreateChildren()
            // declaringBean - the bean just returned by MakeBean()
            void AssignMembers(object declaringBean
                , List<MemberSpec> childrenArg)
            {
                foreach (var memberSpec in childrenArg)
                {
                    if (memberSpec.IsFactory)
                    {
                        try
                        {
                            IOCCFactory factory = memberSpec.MemberOrFactoryBean as IOCCFactory;
                            object memberBean = factory.Execute(new BeanFactoryArgs(
                                memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().FactoryParameter));
                            CreateChildren(memberBean.GetType(), out var memberBeanMembers);
                            AssignMembers(memberBean, memberBeanMembers);
                            memberSpec.FieldOrPropertyInfo.SetValue(declaringBean, memberBean);
                        }
                        catch (ArgumentException ae)
                        {
                            RecordDiagnostic(diagnostics, "TypeMismatch"
                                , ("DeclaringBean", declaringBean.GetType().FullName)
                                , ("Member", memberSpec.FieldOrPropertyInfo.Name)
                                , ("Factory", memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().Factory.FullName)
                                , ("ExpectedType", memberSpec.FieldOrPropertyInfo.MemberType)
                                , ("Exception", ae));
                        }
                        catch (Exception ex)
                        {
                            RecordDiagnostic(diagnostics, "FactoryExecutionFailure"
                                , ("DeclaringBean", declaringBean.GetType().FullName)
                                , ("Member", memberSpec.FieldOrPropertyInfo.Name)
                                , ("Factory", memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().Factory.FullName)
                                , ("Exception", ex));
                        }
                    }
                    else
                    {
                        memberSpec.FieldOrPropertyInfo.SetValue(declaringBean, memberSpec.MemberOrFactoryBean);
                    }

                }
            }
            Type GetFactoryImplementation((Type type, string name) beanIdArg)
            {
                (var factory, var factoryName) = (beanIdArg.type?.GetBeanReferenceAttribute()?.Factory
                  , beanIdArg.type?.GetBeanReferenceAttribute()?.Name);
                if (IsBeanPresntInTypeMap((factory, factoryName)))
                {
                    return GetImplementationFromTypeMap((factory, factoryName));
                }
                return null;
            }
            /*
             * finds the matching concrete type (bean) for some member reference
             * where the member reference might be a base class or interface together
             * with an optional bean name (held as part of the bean reference attribute
             * which allows the container to choose between multiple matching concrete classes
             * Alternatively the member reference may be the implementationType itself.
             */
            (bool complete, Type implementationType) GetImplementationType()
            {
                if (IsBeanPresntInTypeMap(beanId))
                {
                    implementationType = GetImplementationFromTypeMap(beanId);
                    return (false, implementationType);
                }
                else
                {
                    if (beanReferenceDetails.IsRoot)
                    {
                        RecordDiagnostic(diagnostics, "MissingRoot"
                            , ("BeanType", beanId.Item1.GetIOCCName())
                            , ("BeanName", beanId.Item2)
                        );
                        throw new IOCCException("failed to create object tree - see diagnostics for detail",
                            diagnostics);
                    }
                    else
                    {
                        RecordDiagnostic(diagnostics, "MissingBean"
                            , ("Bean", beanReferenceDetails.DeclaringType.GetIOCCName())
                            , ("MemberType", beanId.Item1.GetIOCCName())
                            , ("MemberName", beanReferenceDetails.MemberName)
                            , ("MemberBeanName", beanReferenceDetails.MemberBeanName)
                        );
                        return (true, null);
                    }
                }
            }

            bool complete;
            bool hasFactory;
            object bean;
            (complete, implementationType) = GetImplementationType();
            if (complete)
            {
                return null;
            }
            CreateChildren(implementationType, out var children);
            (complete, bean) = MakeBean();
            if (complete)
            {
                return bean;        // either the bean and therefore its children had already been created
                                    // or we were unable to create the bean (null)
            }
            AssignMembers(bean, children);
            return bean;
        }


        private static void RecordDiagnostic(IOCCDiagnostics diagnostics, string groupName
            , params (string member, object value)[] occurrences)
        {
            dynamic diag = diagnostics.Groups[groupName].CreateDiagnostic();
            foreach ((var member, var value) in occurrences)
            {
                diag.Members[member] = value;
            }
            diagnostics.Groups[groupName].Add(diag);
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

    internal class MemberSpec
    {
        public MemberInfo FieldOrPropertyInfo { get; }          
        public object MemberOrFactoryBean { get; }
        public bool IsFactory { get; }
        public List<MemberSpec> FactoryCreatedMembers { get; }

        public MemberSpec(MemberInfo fieldOrPropertyInfo
            , object memberOrFactoryBean
            , bool isFactory
            , List<MemberSpec> factoryCreatedMembers)
        {
            this.FieldOrPropertyInfo = fieldOrPropertyInfo;
            this.MemberOrFactoryBean = memberOrFactoryBean;
            this.IsFactory = isFactory;
            this.FactoryCreatedMembers = factoryCreatedMembers;
            //Assert(!isFactory && FactoryCreatedMembers == null 
            //  || isFactory && FactoryCreatedMembers != null);
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
            Assert( memberInfo is FieldInfo || memberInfo is PropertyInfo);
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
    }
}
