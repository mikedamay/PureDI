using System;
using System.CodeDom;
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
            , string rootBeanName, string rootConstructorName, BeanScope scope,
            IDictionary<Type, object> mapObjectsCreatedSoFar)
        {
            try
            {
                Assert(rootType != null);
                Assert(rootBeanName != null);
                var rootObject = CreateObjectTree((rootType, rootBeanName, rootConstructorName)
                    , new CreationContext(mapObjectsCreatedSoFar, new CycleGuard(), new HashSet<Type>())
                    , diagnostics, new BeanReferenceDetails(), scope);
                if (rootObject != null && !rootType.IsInstanceOfType(rootObject))
                {
                    throw new IOCCInternalException(
                        $"object created by IOC container is not {rootType.Name} as expected");
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
                throw new IOCCException("Failed to create object tree - see diagnostics for details", inace,
                    diagnostics);
            }
        }

        /// <summary>
        /// see documentation for GetOrCreateObjectTree
        /// </summary>
        /// <param name="beanId">the type + beanName for which a bean is to be created.
        ///     The bean will not necessarily have the type passed in as this
        ///     may be a base class or interface (constructed generic type)
        ///     from which the bean is derived</param>
        /// <param name="creationContext"></param>
        /// <param name="diagnostics"></param>
        /// <param name="beanReferenceDetails">provides a context to the bean that
        ///     can be displayed in diagnostic messages - currently not used for
        ///     anything else</param>
        /// <param name="beanScope"></param>
        /// <param name="mapObjectsCreatedSoFar">for all beans instantiated to this point
        ///     maps the name of the class or struct of
        ///     the object to the instance of the object.</param>
        /// <param name="fieldOrPropertyInfo1">used to determine the scope of the bean to be created</param>
        /// <param name="bean">a class already instantiated by IOCC whose
        ///                    fields and properties may need to be injuected</param>
        private object CreateObjectTree((Type beanType, string beanName, string constructorName) beanId,
            CreationContext creationContext, IOCCDiagnostics diagnostics, BeanReferenceDetails beanReferenceDetails,
            BeanScope beanScope)
        {
            Type MakeConstructableType((Type beanType, string beanName, string constructorName) beanIdArg,
                Type implementationTypeArg)
                => implementationTypeArg.IsGenericType
                    ? beanIdArg.beanType
                    : implementationTypeArg;

            Type implementationType;

            (bool constructionComplete, object beanId) MakeBean(IList<ChildBeanSpec> constructorParameterSpecs = null)
            {            
                object constructedBean;
                try
                {
                    Type constructableTypeLocal = MakeConstructableType(beanId, implementationType);
                    if (beanScope != BeanScope.Prototype
                        && creationContext.MapObjectsCreatedSoFar.ContainsKey(constructableTypeLocal))
                    {
                        // there maybe a cyclical dependency
                        constructedBean = creationContext.MapObjectsCreatedSoFar[constructableTypeLocal];
                        return (true, constructedBean);
                    }
                    else
                    {
                        // TODO explain why type to be constructed is complicated by generics
                        constructedBean = Construct(constructableTypeLocal
                          , constructorParameterSpecs, beanId.constructorName);
                        if (beanScope != BeanScope.Prototype)
                        {
                            creationContext.MapObjectsCreatedSoFar[constructableTypeLocal] = constructedBean;
                            // TODO replace first param with ConstructableType
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
            } // MakeBean()

            void CreateTreeForMemberOrParameter(Info fieldOrPropertyInfo, Type declaringBeanType, List<ChildBeanSpec> members)
            {
                IOCCBeanReferenceAttribute attr;
                if ((attr = fieldOrPropertyInfo.GetCustomeAttribute<IOCCBeanReferenceAttribute>()) != null)
                {
                    //Assert(fieldOrPropertyInfo is FieldInfo
                    //       || fieldOrPropertyInfo is PropertyInfo);
                    (Type type, string beanName, string constructorName) memberBeanId =
                        MakeMemberBeanId(fieldOrPropertyInfo.Type
                            , attr.Name, attr.ConstructorName, beanId);
                    object memberBean;
                    if (!fieldOrPropertyInfo.IsWriteable)
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
                                o = CreateObjectTree((attr.Factory, attr.Name, attr.ConstructorName), creationContext
                                    , diagnostics
                                    , new BeanReferenceDetails(declaringBeanType
                                        , fieldOrPropertyInfo.Name, memberBeanId.beanName), attr.Scope);
                            if (o == null)
                            {
                                RecordDiagnostic(diagnostics, "MissingFactory"
                                    , ("DeclaringBean", declaringBeanType.FullName)
                                    , ("Member", fieldOrPropertyInfo.Name)
                                    , ("Factory", attr.Factory.FullName)
                                    , ("ExpectedType", fieldOrPropertyInfo.Type));
                            }
                            else if (!(o is IOCCFactory))
                            {
                                RecordDiagnostic(diagnostics, "BadFactory"
                                    , ("DeclaringBean", declaringBeanType.FullName)
                                    , ("Member", fieldOrPropertyInfo.Name)
                                    , ("Factory", attr.Factory.FullName)
                                );
                            }
                            else // factory successfully created
                            {
                                IOCCFactory factoryBean = (o as IOCCFactory);
                                members.Add(new ChildBeanSpec(fieldOrPropertyInfo, factoryBean, true));
                            }
                        }
                        else // create the member without using a factory
                        {
                            memberBean = CreateObjectTree(memberBeanId, creationContext
                                , diagnostics
                                , new BeanReferenceDetails(declaringBeanType
                                    , fieldOrPropertyInfo.Name, memberBeanId.beanName), attr.Scope);
                            members.Add(new ChildBeanSpec(fieldOrPropertyInfo, memberBean, false));
                        } // not a factory
                    } // writeable member
                } // this is a bean reference
            }

            void CreateMemberTrees(Type declaringBeanType
                , out List<ChildBeanSpec> members)
            {
                members = new List<ChildBeanSpec>();
                var fieldOrPropertyInfos = declaringBeanType.GetMembers(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(f => f is FieldInfo || f is PropertyInfo);
                foreach (var fieldOrPropertyInfo in fieldOrPropertyInfos)
                {
                    CreateTreeForMemberOrParameter(new Info(fieldOrPropertyInfo), declaringBeanType, members);
                } // for each property or field
            } // CreateMemberTrees()


            ParameterInfo[] GetParametersForConstructorMatching(Type declaringBeanTypeArg, string constructorNameArg)
            {
                Type dt = declaringBeanTypeArg;
                return declaringBeanTypeArg
                    .GetConstructors().FirstOrDefault(co
                        => co.GetCustomAttribute<IOCCConstructorAttribute>() != null
                           && co.GetCustomAttribute<IOCCConstructorAttribute>()?
                               .Name == constructorNameArg)?.GetParameters();
            }

            void CreateConstructorTrees(Type declaringBeanType
                , out List<ChildBeanSpec> members)
            {
                members = new List<ChildBeanSpec>();
                string constructorName = declaringBeanType.GetConstructorNameFromMember();
                if (declaringBeanType.GetConstructors().Length > 0)
                {
                    var paramInfos = GetParametersForConstructorMatching(declaringBeanType, constructorName);
                    if (paramInfos != null)
                    {
                        foreach (var paramInfo in paramInfos)
                        {
                            CreateTreeForMemberOrParameter(new Info(paramInfo), declaringBeanType, members);
                        } // for each constructor parameter
                    }

                }
            } // CreateMemberTrees()

            // declaringBean - the bean just returned by MakeBean()
            void AssignMembers(object declaringBean
                , List<ChildBeanSpec> childrenArg)
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
                            CreateMemberTrees(memberBean.GetType(), out var memberBeanMembers);
                            AssignMembers(memberBean, memberBeanMembers);
                            memberSpec.FieldOrPropertyInfo.SetValue(declaringBean, memberBean);
                        }
                        catch (ArgumentException ae)
                        {
                            RecordDiagnostic(diagnostics, "TypeMismatch"
                                , ("DeclaringBean", declaringBean.GetType().FullName)
                                , ("Member", memberSpec.FieldOrPropertyInfo.Name)
                                , ("Factory", memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().Factory
                                    .FullName)
                                , ("ExpectedType", memberSpec.FieldOrPropertyInfo.MemberType)
                                , ("Exception", ae));
                        }
                        catch (Exception ex)
                        {
                            RecordDiagnostic(diagnostics, "FactoryExecutionFailure"
                                , ("DeclaringBean", declaringBean.GetType().FullName)
                                , ("Member", memberSpec.FieldOrPropertyInfo.Name)
                                , ("Factory", memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().Factory
                                    .FullName)
                                , ("Exception", ex));
                        }
                    }
                    else
                    {
                        memberSpec.FieldOrPropertyInfo.SetValue(declaringBean, memberSpec.MemberOrFactoryBean);
                    }

                }
            }

            /*
             * finds the matching concrete type (bean) for some member reference
             * where the member reference might be a base class or interface together
             * with an optional bean name (held as part of the bean reference attribute
             * which allows the container to choose between multiple matching concrete classes
             * Alternatively the member reference may be the implementationType itself.
             */
            Type GetImplementationType()
            {
                if (IsBeanPresntInTypeMap(beanId))
                {
                    implementationType = GetImplementationFromTypeMap(beanId);
                    return implementationType;
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
                        return null;
                    }
                }
            }

            CycleGuard cycleGuard = creationContext.CycleGuard;
            ISet<Type> cyclicalDependencies = creationContext.CyclicalDependencies;
            bool complete;
            object bean;
            if ((implementationType = GetImplementationType()) == null)
            {
                return null; // no implementation type found corresponding to this beanId
                // TODO don't we need some diangostics here
            }
            Type constructableType = MakeConstructableType(beanId, implementationType);
            bool cyclicalDependencyFound = false;
            try
            {
                cyclicalDependencyFound = cycleGuard.IsPresent(constructableType);
                if (!cyclicalDependencyFound)
                {
                    cycleGuard.Push(constructableType);
                    CreateMemberTrees(constructableType, out var memberSpecs);
                    CreateConstructorTrees(constructableType, out var parameterSpec);
                    (complete, bean) = MakeBean(parameterSpec);
                    Assert(!cyclicalDependencies.Contains(constructableType)
                           || cyclicalDependencies.Contains(constructableType)
                           && complete
                           && bean != null); // "complete && bean != null" indicates that
                    // MakeBean found a bean cached in mapObjectsCreatedSoFar.
                    // 
                    // if a cyclical dependency was found lower
                    // in the stack then a bean must have been
                    // created for it at that time. So wtf!
                    if (complete && !cyclicalDependencies.Contains(constructableType))
                    {
                        return bean; // either the bean and therefore its children had already been created
                        // or we were unable to create the bean (null)
                    }
                    cyclicalDependencies.Remove(constructableType);
                    AssignMembers(bean, memberSpecs);
                }
                else // there is a cyclical dependency so we
                    // need to just create the bean itself and defer child creation
                    // until the implementationType is encountered again
                    // further up the stack
                {
                    if (constructableType.HasInjectedConstructorParameters(IOCC.DEFAULT_CONSTRUCTOR_NAME))
                    {
                        throw new IOCCException("Cannot create this bean due to a cyclical dependency", diagnostics);
                    }
                    (complete, bean) = MakeBean();
                    if (bean != null)
                    {
                        cyclicalDependencies.Add(constructableType);
                    }
                }
            }
            finally
            {
                if (!cyclicalDependencyFound)
                {
                    cycleGuard.Pop();
                }
            }
            return bean;
        }


        /// <summary>
        /// well, this is tricky.
        /// </summary>
        /// <param name="memberDeclaredBeanType"></param>
        /// <param name="memberDeclaredBeanName"></param>
        /// <param name="declaringBeanId"></param>
        /// <returns></returns>
        (Type type, string beanName, string constructorName)
            MakeMemberBeanId(Type memberDeclaredBeanType
                , string memberDeclaredBeanName, string constructorName
                , (Type type, string beanName, string constructorName) declaringBeanId)
        {
            Assert(!memberDeclaredBeanType.IsGenericParameter);
            return (memberDeclaredBeanType, memberDeclaredBeanName, constructorName);
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
        private Type GetImplementationFromTypeMap((Type beanType, string beanName, string constructorName) beanId)
        {

            char[] a = beanId.beanType.FullName.TakeWhile(n => n != '[').ToArray();
            string beanTypeName = new String(a);
            // trim the generic arguments from a generic
            Type referenceType = typeMap.Keys.FirstOrDefault(
                k => k.type.FullName == beanTypeName && k.beanName == beanId.beanName).type;
            return typeMap[(referenceType, beanId.beanName)];

        }

        /// <param name="beanid"><see cref="GetImplementationFromTypeMap"/></param>
        private bool IsBeanPresntInTypeMap((Type beanType, string beanName, string constructorName) beanId)
        {
            char[] a = beanId.beanType.FullName.TakeWhile(n => n != '[').ToArray();
            string beanTypeName = new String(a);
            // trim the generic arguments from a generic
            return typeMap.Keys.Any(k => k.type.FullName == beanTypeName && k.beanName == beanId.beanName);
        }

        /// <summary>checks if the type to be instantiated has an empty constructor and if so constructs it</summary>
        /// <param name="beanType">a concrete clasws typically part of the object tree being instantiated</param>
        /// <exception>InvalidArgumentException</exception>  
        private object Construct(Type beanType, IList<ChildBeanSpec> constructorParameterSpecs, string constructorName)
        {
            object[] args = new object[0];
            if (beanType.IsStruct())
            {
                return Activator.CreateInstance(beanType);
            }
            else
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                ConstructorInfo constructorInfo;
                if (constructorParameterSpecs?.Count > 0)
                {
                    constructorInfo = beanType.GetConstructorNamed(constructorName);
                    if (constructorInfo == null)
                    {
                        throw new IOCCInternalException("gone badly wrong");
                        // TODO diagnostics and a good exception required here
                    }
                    List<object> parameters = new List<object>();
                    foreach (var spec in constructorParameterSpecs)
                    {
                        if (spec.IsFactory)
                        {
                            // TODO
                        }
                        else
                        {
                            parameters.Add(spec.MemberOrFactoryBean);   
                        }
                    }
                    args = parameters.ToArray();
                }
                else
                {
                    var constructorInfos = beanType.GetConstructors(flags);
                    constructorInfo = constructorInfos.FirstOrDefault(ci => ci.GetParameters().Length == 0);
                }
                if (constructorInfo == null)
                {
                    throw new IOCCNoArgConstructorException(beanType.GetIOCCName());
                    // TODO some diagnostics required here
                }
                return constructorInfo.Invoke(flags | BindingFlags.CreateInstance, null, args, null);

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

        internal class CreationContext
        {
            public IDictionary<Type, object> MapObjectsCreatedSoFar { get; }
            public CycleGuard CycleGuard { get; }
            public ISet<Type> CyclicalDependencies { get; }

            public CreationContext(
                IDictionary<Type, object> mapObjectsCreatedSoFar
                , CycleGuard cycleGuard
                , ISet<Type>cyclicalDependencies
            )
            {
                MapObjectsCreatedSoFar = mapObjectsCreatedSoFar;
                CycleGuard = cycleGuard;
                CyclicalDependencies = cyclicalDependencies;
            }
        }
    }

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

    internal class ChildBeanSpec
    {
        public MemberInfo FieldOrPropertyInfo
        {
            get { return Info.FieldOrPropertyInfo; }
        }

        public ParameterInfo ParameterInfo
        {
            get { return Info.ParameterInfo; }
        }
        public object MemberOrFactoryBean { get; }
        public bool IsFactory { get; }
        private Info Info { get; }

        public ChildBeanSpec(Info fieldOrPropertyInfo, object memberOrFactoryBean, bool isFactory)
        {
            this.Info = fieldOrPropertyInfo;
            this.MemberOrFactoryBean = memberOrFactoryBean;
            this.IsFactory = isFactory;
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

        public static bool HasInjectedConstructorParameters(this Type type, string constructorName)
        {
            return type.GetConstructors().Any(c => c.GetCustomAttributes().Any(
                ca => ca is IOCCConstructorAttribute
                      && (ca as IOCCConstructorAttribute).Name == constructorName));
        }

        public static string GetConstructorNameFromMember(this Type type)
        {
            IOCCBeanReferenceAttribute attr = type.GetCustomAttribute<IOCCBeanReferenceAttribute>();
            if (attr != null)
            {
                return attr.ConstructorName;
            }
            return IOCC.DEFAULT_CONSTRUCTOR_NAME;
        }

        public static ConstructorInfo GetConstructorNamed(this Type type, string name)
        {
            return type.GetConstructors()
              .FirstOrDefault(co => co.GetCustomAttribute<
              IOCCConstructorAttribute>() != null
              && co.GetCustomAttribute<
              IOCCConstructorAttribute>().Name == name);
        }
    }
}
