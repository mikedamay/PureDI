using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PureDI.Common;
using PureDI.Attributes;
using static PureDI.Common.Common;

namespace PureDI.Tree
{
    internal class ObjectTree
    {
        private const BindingFlags constructorFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public ObjectTree()
        {
        }

        // TODO complete the documentation item 3 below if and when factory types are implemented
        /// <summary>
        /// 1. mainly used to create the complete object tree at program startup
        /// 2. may be used to create object tree fragments when running tests
        /// 3. may be used to create an object or link to an existing object
        /// </summary>
        /// <param name="rootType">The top node in the tree</param>
        /// <param name="injectionState"></param>
        /// <param name="rootBeanName"></param>
        /// <param name="rootConstructorName"></param>
        /// <param name="scope"></param>
        /// <param name="mapObjectsCreatedSoFar"></param>
        /// <returns>an ojbect of root type</returns>
        public (object bean, InjectionState injectionState) 
          CreateAndInjectDependencies(Type rootType, InjectionState injectionState, string rootBeanName
            ,string rootConstructorName, BeanScope scope, IDictionary<(Type, string), object> mapObjectsCreatedSoFar)
        {
            try
            {
                Assert(rootType != null);
                Assert(rootBeanName != null);
                object rootObject;
                (rootObject, injectionState) = CreateObjectTree((rootType, rootBeanName, rootConstructorName)
                    , new CreationContext(new CycleGuard(), new HashSet<Type>()), injectionState, new BeanReferenceDetails(), scope);
                if (rootObject != null && !rootType.IsInstanceOfType(rootObject))
                {
                    throw new DIException(
                        $"object created by IOC container is not {rootType.Name} as expected", injectionState.Diagnostics);
                }
                Assert(rootObject == null
                       || rootType.IsInstanceOfType(rootObject));
                return (rootObject, injectionState);
            }
            catch (NoArgConstructorException inace)
            {
                dynamic diagnostic = injectionState.Diagnostics.Groups["MissingNoArgConstructor"].CreateDiagnostic();
                diagnostic.Class = rootType.GetIOCCName();
                injectionState.Diagnostics.Groups["MissingNoArgConstructor"].Add(diagnostic);
                throw new DIException("Failed to create object tree - see diagnostics for details", inace,
                    injectionState.Diagnostics);
            }
        }
        public InjectionState CreateAndInjectDependencies(object rootObject, InjectionState injectionState)
        {
            CreationContext cc = new CreationContext(new CycleGuard(), new HashSet<Type>());
            CreateMemberTrees(rootObject.GetType(), out var memberSpecs, cc, injectionState);
            return AssignMembers(rootObject, memberSpecs, injectionState, cc);

        }

        /// <summary>
        /// see documentation for CreateAndInjectDependencies
        /// </summary>
        /// <param name="beanId">the type + beanName for which a bean is to be created.
        ///     The bean will not necessarily have the type passed in as this
        ///     may be a base class or interface (constructed generic type)
        ///     from which the bean is derived</param>
        /// <param name="creationContext"></param>
        /// <param name="injectionState"></param>
        /// <param name="beanReferenceDetails">provides a context to the bean that
        ///     can be displayed in diagnostic messages - currently not used for
        ///     anything else</param>
        /// <param name="beanScope"></param>
        private (object bean, InjectionState injectionState) 
          CreateObjectTree((Type beanType, string beanName
          , string constructorName) beanId, CreationContext creationContext
          , InjectionState injectionState, BeanReferenceDetails beanReferenceDetails
          , BeanScope beanScope)
        {
            Type implementationType;

            (bool constructionComplete, object beanId) 
              MakeBean(IList<ChildBeanSpec> constructorParameterSpecs = null)
            {            
                object constructedBean;
                try
                {
                    Type constructableTypeLocal = MakeConstructableType(beanId, implementationType);
                    if (beanScope != BeanScope.Prototype
                        && injectionState.MapObjectsCreatedSoFar.ContainsKey((constructableTypeLocal, beanId.constructorName)))
                    {
                        // there maybe a cyclical dependency
                        constructedBean = injectionState.MapObjectsCreatedSoFar[(constructableTypeLocal, beanId.constructorName)];
                        return (true, constructedBean);
                    }
                    else
                    {
                        // TODO explain why type to be constructed is complicated by generics
                        (constructedBean, injectionState) = Construct(constructableTypeLocal
                            , constructorParameterSpecs, beanId.constructorName, injectionState);
                        if (beanScope != BeanScope.Prototype)
                        {
                            injectionState.MapObjectsCreatedSoFar[(constructableTypeLocal, beanId.constructorName)] = constructedBean;
                            // TODO replace first param with ConstructableType
                        }
                    }
                }
                catch (NoArgConstructorException inace)
                {
                    RecordDiagnostic(injectionState.Diagnostics, "MissingNoArgConstructor"
                        , ("Class", inace.Class));
                    return (true, null);
                }
                return (false, constructedBean);
            } // MakeBean()

            // throws an exception for invalid constructors.
            // With member references there's a good chance that
            // we can hang around to collect more
            // diagnostics but with constructors hanging around is
            // more liable to cause chaos
            void CreateConstructorTrees(Type declaringBeanType
                , out List<ChildBeanSpec> members)
            {
                members = new List<ChildBeanSpec>();
                string constructorName = beanId.Item3;
                ValidateConstructors(declaringBeanType, constructorName
                  , injectionState.Diagnostics, beanReferenceDetails);
                if (declaringBeanType.GetConstructors(
                  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                  .Length > 0)
                {
                    var paramInfos = GetParametersForConstructorMatching(declaringBeanType, constructorName);
                    if (paramInfos != null)
                    {
                        foreach (var paramInfo in paramInfos)
                        {
                            CreateTreeForMemberOrParameter(new Info(paramInfo), declaringBeanType, members
                              , creationContext, injectionState);
                        } // for each constructor parameter
                    }

                }
            } // CreateConstructorTrees()

            CycleGuard cycleGuard = creationContext.CycleGuard;
            ISet<Type> cyclicalDependencies = creationContext.CyclicalDependencies;
            bool complete;
            object bean;
            if (((implementationType, injectionState) 
              = GetImplementationType(beanId, beanReferenceDetails, injectionState)).implementationType == null)
            {
                Assert(injectionState.Diagnostics.HasWarnings);    // diagnostics recorded by callee.
                return (null, injectionState); // no implementation type found corresponding to this beanId
                             // we can still carry on and a) this might not be fatal b) other diagnostics may show up
                            
            }
            Type constructableType = MakeConstructableType(beanId, implementationType);
            bool cyclicalDependencyFound = false;
            try
            {
                cyclicalDependencyFound = cycleGuard.IsPresent(constructableType);
                if (!cyclicalDependencyFound)
                {
                    cycleGuard.Push(constructableType);
                    CreateMemberTrees(constructableType, out var memberSpecs, creationContext, injectionState);
                    CreateConstructorTrees(constructableType, out var parameterSpecs);
                    (complete, bean) = MakeBean(parameterSpecs);
                    Assert(!cyclicalDependencies.Contains(constructableType)
                           || cyclicalDependencies.Contains(constructableType)
                           && complete
                           && bean != null); 
                            // "complete && bean != null" indicates that
                            // MakeBean found a bean cached in mapObjectsCreatedSoFar.
                            // 
                            // if a cyclical dependency was found lower
                            // in the stack then a bean must have been
                            // created for it at that time. So wtf!
                    // There are 3 cases where complete == true (i.e. attempted bean instantiation is complete)
                    // 1) bean instantiation failed; in which case we will return null - caller can deal
                    // 2) bean already existed (all of its members will have already been created and assigned).
                    // 3) bean already existed but because it was a cyclical dependency its
                    //    members had not been created and assigned and that needs to be done now.
                    //    (the MakeBean() logic cannot distinguish between 2) and 3) so, in the
                    //     case of 3) it wrongly reports that the bean creation is complete)
                    if (complete && !cyclicalDependencies.Contains(constructableType))
                    {
                        return (bean, injectionState); // either the bean and therefore its children had already been created
                                     // or we were unable to create the bean (null)
                    }
                    cyclicalDependencies.Remove(constructableType);
                    AssignMembers(bean, memberSpecs, injectionState, creationContext);
                }
                else // there is a cyclical dependency so we
                    // need to just create the bean itself and defer child creation
                    // until the implementationType is encountered again
                    // further up the stack
                {
                    if (constructableType.HasInjectedConstructorParameters(Constants.DefaultConstructorName))
                    {
                        dynamic diag = injectionState.Diagnostics.Groups["CyclicalDependency"].CreateDiagnostic();
                        diag.Bean = constructableType.FullName;
                        injectionState.Diagnostics.Groups["CyclicalDependency"].Add(diag);
                        throw new DIException("Cannot create this bean due to a cyclical dependency", injectionState.Diagnostics);
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
            return (bean, injectionState);
        }       // CreateObjectTree

        void CreateMemberTrees(Type declaringBeanType, out List<ChildBeanSpec> members, CreationContext creationContext, InjectionState injectionState)
        {
            members = new List<ChildBeanSpec>();
            var fieldOrPropertyInfos = declaringBeanType.GetMembers(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f is FieldInfo || f is PropertyInfo);
            foreach (var fieldOrPropertyInfo in fieldOrPropertyInfos)
            {
                CreateTreeForMemberOrParameter(new Info(fieldOrPropertyInfo), declaringBeanType
                    , members, creationContext, injectionState);
            } // for each property or field
        } // CreateMemberTrees()

        private InjectionState CreateTreeForMemberOrParameter(Info fieldOrPropertyInfo, Type declaringBeanType, List<ChildBeanSpec> members, CreationContext creationContext, InjectionState injectionState)
        {
                 BeanReferenceBaseAttribute attr;
                if ((attr = fieldOrPropertyInfo.GetCustomeAttribute<BeanReferenceBaseAttribute>()) != null)
                {
                    (Type type, string beanName, string constructorName) memberBeanId =
                        MakeMemberBeanId(fieldOrPropertyInfo.Type
                            , attr.Name, attr.ConstructorName);
                    object memberBean;
                    if (!fieldOrPropertyInfo.IsWriteable)
                    {
                        RecordDiagnostic(injectionState.Diagnostics, "ReadOnlyProperty"
                            , ("Class", declaringBeanType.GetIOCCName())
                            , ("Member", fieldOrPropertyInfo.Name));
                    }
                    else // member is writable
                    {
                        if (attr.Factory != null)
                        {
                            // create the factory
                            object o;
                                (o, injectionState) = CreateObjectTree((attr.Factory, attr.Name, attr.ConstructorName), creationContext, injectionState, new BeanReferenceDetails(declaringBeanType
                                    , fieldOrPropertyInfo.Name, memberBeanId.beanName), attr.Scope);
                            if (o == null)
                            {
                                RecordDiagnostic(injectionState.Diagnostics, "MissingFactory"
                                    , ("DeclaringBean", declaringBeanType.FullName)
                                    , ("Member", fieldOrPropertyInfo.Name)
                                    , ("Factory", attr.Factory.FullName)
                                    , ("ExpectedType", fieldOrPropertyInfo.Type));
                            }
                            else if (!(o is IFactory))
                            {
                                RecordDiagnostic(injectionState.Diagnostics, "BadFactory"
                                    , ("DeclaringBean", declaringBeanType.FullName)
                                    , ("Member", fieldOrPropertyInfo.Name)
                                    , ("Factory", attr.Factory.FullName)
                                );
                            }
                            else // factory successfully created
                            {
                                IFactory factoryBean = (o as IFactory);
                                members.Add(new ChildBeanSpec(fieldOrPropertyInfo, factoryBean, true));
                            }
                        }
                        else // create the member without using a factory
                        {
                            (memberBean, injectionState) = CreateObjectTree(memberBeanId, creationContext, injectionState, new BeanReferenceDetails(declaringBeanType
                                , fieldOrPropertyInfo.Name, memberBeanId.beanName), attr.Scope);
                            members.Add(new ChildBeanSpec(fieldOrPropertyInfo, memberBean, false));
                        } // not a factory
                    } // writeable member
                } // this is a bean reference
            return injectionState;
        }
        // declaringBean - the bean just returned by MakeBean()
        InjectionState AssignMembers(object declaringBean, List<ChildBeanSpec> childrenArg, InjectionState injectionState, CreationContext creationContext)
        {
            void AssignBean(ChildBeanSpec memberSpec, object memberBean)
            {
                if (memberBean != null)
                {
                    if (memberSpec.FieldOrPropertyInfo.CanReadFromFieldOrProperty())
                    {
                        object existingValue = memberSpec.FieldOrPropertyInfo.GetValue(declaringBean);
                        if (existingValue != null && existingValue.ToString() != "0")
                        {
                            RecordDiagnostic(injectionState.Diagnostics, "AlreadyInitialised"
                                , ("DeclaringType", declaringBean.GetType().FullName)
                                , ("Member", memberSpec.FieldOrPropertyInfo.Name)
                                , ("ExistingValue", existingValue.ToString())
                            );
                        }
                    }
                    memberSpec.FieldOrPropertyInfo.SetValue(declaringBean, memberBean);
                    LogMemberInjection(injectionState.Diagnostics, declaringBean.GetType()
                        , memberSpec.FieldOrPropertyInfo.GetDeclaredType()
                        , memberSpec.FieldOrPropertyInfo.Name
                        , memberBean.GetType());
                }
            }
            foreach (var memberSpec in childrenArg)
            {
                object memberBean = default;
                if (memberSpec.IsFactory)
                {
                    try
                    {
                        IFactory factory = memberSpec.MemberOrFactoryBean as IFactory;
                        try
                        {
                            (memberBean, injectionState) = factory.Execute(injectionState, new BeanFactoryArgs(
                                                memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().FactoryParameter));
                        }
                        catch (Exception ex)
                        {
                            throw new DIException($"Execute failed for {factory.GetType().FullName}", ex, injectionState.Diagnostics);
                        }
                        CreateMemberTrees(memberBean.GetType(), out var memberBeanMembers, creationContext, injectionState);
                        AssignMembers(memberBean, memberBeanMembers, injectionState, creationContext);
                        AssignBean(memberSpec, memberBean);
                    }
                    catch (ArgumentException ae)
                    {
                        RecordDiagnostic(injectionState.Diagnostics, "TypeMismatch"
                            , ("DeclaringBean", declaringBean.GetType().FullName)
                            , ("Member", memberSpec.FieldOrPropertyInfo.Name)
                            , ("Factory", memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().Factory
                                .FullName)
                            , ("ExpectedType", memberSpec.FieldOrPropertyInfo.MemberType)
                            , ("Exception", ae));
                    }
                    catch (Exception ex)
                    {
                        RecordDiagnostic(injectionState.Diagnostics, "FactoryExecutionFailure"
                            , ("DeclaringBean", declaringBean.GetType().FullName)
                            , ("Member", memberSpec.FieldOrPropertyInfo.Name)
                            , ("Factory", memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().Factory
                                .FullName)
                            , ("Exception", ex));
                        throw;
                    }
                }
                else    // non-factory
                {
                    memberBean = memberSpec.MemberOrFactoryBean;
                    AssignBean(memberSpec, memberBean);
                }
            }       // foreach memberSpec
            return injectionState;
        }  // AssignMembers()


        private void LogMemberInjection(Diagnostics diagnostics, Type declarngType
          , Type declaredType, string memberName, Type memberImplementation)
        {
            Diagnostics.Group group = diagnostics.Groups["MemberInjectionsInfo"];
            dynamic diag = group.CreateDiagnostic();
            diag.DeclaringType = declarngType;
            diag.MemberType = declaredType;
            diag.MemberName = memberName;
            diag.MemberImplementation = memberImplementation;
            group.Add(diag);
        }

        /// <summary>
        /// errros if: 
        ///     a) multiple candidate constructors
        ///     b) missing parameters
        /// warns if:
        ///     there are parameters marked as bean references
        ///     for constructors that aren't marked
        /// </summary>
        /// <param name="declaringBeanType">whose constructor are we talking about</param>
        /// <param name="constructorName">which of a number of competing construcors are we talking about</param>
        /// <param name="diagnostics"></param>
        /// <param name="beanReferenceDetails">context about the member referring to this bean+constructor</param>
        private void ValidateConstructors(Type declaringBeanType
          ,string constructorName, Diagnostics diagnostics
          ,BeanReferenceDetails beanReferenceDetails)
        {
            ConstructorInfo[] constructors
              = declaringBeanType.GetConstructors(constructorFlags
              ).Where(co => co.GetCustomAttributes<ConstructorBaseAttribute>()
              .Any(ca => ca.Name == constructorName)).ToArray();
            if (declaringBeanType.GetConstructors().Where(
                    co => !co.GetCustomAttributes<ConstructorBaseAttribute>().Any())
                .Any(co => co.GetParameters().Any(
                    p => p.GetCustomAttributes<BeanReferenceBaseAttribute>().Any())))
            {
                dynamic diag = diagnostics.Groups["MissingConstructorAttribute"].CreateDiagnostic();
                diag.Bean = declaringBeanType;
                diagnostics.Groups["MissingConstructorAttribute"].Add(diag);
            }
            if (constructors.Length == 0)
            {
                return;
            }
            if (constructors.Length > 1)
            {
                dynamic diag = diagnostics.Groups["DuplicateConstructors"].CreateDiagnostic();
                diag.Bean = declaringBeanType;
                diagnostics.Groups["DuplicateConstructors"].Add(diag);
                throw new DIException($"{declaringBeanType} has duplicate constructors - please see diagnostics for further details",diagnostics);
            }
            if (constructors.Length > 0)
            {
                ConstructorInfo constructor = constructors[0];
                if (constructor.GetParameters().Length > 0
                    && !constructor.GetParameters().All(p => p.GetCustomAttributes<BeanReferenceBaseAttribute>().Any()))
                {
                    dynamic diag = diagnostics.Groups["MissingConstructorParameterAttribute"].CreateDiagnostic();
                    diag.Bean = declaringBeanType;
                    diagnostics.Groups["MissingConstructorParameterAttribute"].Add(diag);
                    throw new DIException($"{declaringBeanType}'s constructor has parameters not marked as [IOCCBeanReference] - please see diagnostics for further details", diagnostics);
                }
            }
        }

        ParameterInfo[] GetParametersForConstructorMatching(
            Type declaringBeanTypeArg, string constructorNameArg)
            => declaringBeanTypeArg
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).FirstOrDefault(co
                    => co.GetCustomAttribute<ConstructorBaseAttribute>() != null
                       && string.Compare(co.GetCustomAttribute<ConstructorBaseAttribute>()?
                           .Name, constructorNameArg, StringComparison.OrdinalIgnoreCase) == 0)?.GetParameters();
        Type MakeConstructableType((Type beanType, string beanName, string constructorName) beanIdArg,
            Type implementationTypeArg)
            => implementationTypeArg.IsGenericType
                ? beanIdArg.beanType
                : implementationTypeArg;

        /*
          * finds the matching concrete type (bean) for some member reference
          * where the member reference might be a base class or interface together
          * with an optional bean name (held as part of the bean reference attribute
          * which allows the container to choose between multiple matching concrete classes
          * Alternatively the member reference may be the implementationType itself.
          */
        (Type implementationType, InjectionState injectionState) 
          GetImplementationType((Type, string, string) beanId
          , BeanReferenceDetails beanReferenceDetails, InjectionState injectionState)
        {
            (bool found, InjectionState injectionState) result;
            if ((result = IsBeanPresntInTypeMap(beanId, injectionState)).found)
            {
                injectionState = result.injectionState;
                Type implementationType;
                (implementationType, injectionState) = GetImplementationFromTypeMap(beanId, injectionState);
                return (implementationType, injectionState);
            }
            else
            {
                injectionState = result.injectionState;
                if (beanReferenceDetails.IsRoot)
                {
                    RecordDiagnostic(injectionState.Diagnostics, "MissingRoot"
                        , ("BeanType", beanId.Item1.GetIOCCName())
                        , ("BeanName", beanId.Item2)
                    );
                    throw new DIException("failed to create object tree - see diagnostics for detail",
                        injectionState.Diagnostics);
                }
                else
                {
                    RecordDiagnostic(injectionState.Diagnostics, "MissingBean"
                        , ("Bean", beanReferenceDetails.DeclaringType.GetIOCCName())
                        , ("MemberType", beanId.Item1.GetIOCCName())
                        , ("MemberName", beanReferenceDetails.MemberName)
                        , ("MemberBeanName", beanReferenceDetails.MemberBeanName)
                    );
                    return (null, injectionState);
                }
            }
        }

        /// <summary>
        /// well, this is tricky.
        /// </summary>
        /// <param name="memberDeclaredBeanType"></param>
        /// <param name="memberDeclaredBeanName"></param>
        /// <param name="constructorName"></param>
        /// <returns></returns>
        private (Type type, string beanName, string constructorName)
            MakeMemberBeanId(Type memberDeclaredBeanType
                , string memberDeclaredBeanName, string constructorName
                )
        {
            Assert(!memberDeclaredBeanType.IsGenericParameter);
            return (memberDeclaredBeanType, memberDeclaredBeanName, constructorName);
        }


        private static void RecordDiagnostic(Diagnostics diagnostics, string groupName
            , params (string member, object value)[] occurrences)
        {
            dynamic diag = diagnostics.Groups[groupName].CreateDiagnostic();
            foreach ((var member, var value) in occurrences)
            {
                diag.Members[member] = value;
            }
            diagnostics.Groups[groupName].Add(diag);
        }

        /// <param name="injectionState"></param>
        /// <param name="beanId">Typically this is the type of a member 
        ///     marked as a bean reference with [IOCCBeanReference]
        ///     for generics bean type is a generic type definition</param>
        /// <returns>This will be a concrete class marked as a bean with [Bean] which
        ///     is derived from the beanId.beanType.  For generics this will be a
        ///     constructed generic type</returns>
        private (Type implementationType, InjectionState injectionState) 
          GetImplementationFromTypeMap((Type beanType, string beanName
          , string constructorName) beanId, InjectionState injectionState)
        {

            char[] a = beanId.beanType.FullName.TakeWhile(n => n != '[').ToArray();
            string beanTypeName = new String(a);
            // trim the generic arguments from a generic
            Type referenceType = injectionState.TypeMap.Keys.FirstOrDefault(
                k => k.beanType.FullName == beanTypeName && k.beanName == beanId.beanName).beanType;
            return (injectionState.TypeMap[(referenceType, beanId.beanName)], injectionState);

        }

        /// <param name="injectionState"></param>
        /// <param name="beanId"><see cref="GetImplementationFromTypeMap"/></param>
        private (bool typeFound, InjectionState injectionState) 
          IsBeanPresntInTypeMap((Type beanType, string beanName
          , string constructorName) beanId, InjectionState injectionState)
        {
            char[] a = beanId.beanType.IsArray 
              ? beanId.beanType.FullName.ToArray() 
              : beanId.beanType.FullName.TakeWhile(n => n != '[').ToArray();
            string beanTypeName = new String(a);
            // trim the generic arguments from a generic
            return (injectionState.TypeMap.Keys.Any(k => k.beanType.FullName 
              == beanTypeName && k.beanName == beanId.beanName), injectionState);
        }

        /// <summary>checks if the type to be instantiated has a valid constructor and if so constructs it</summary>
        /// <param name="beanType">a concrete clasws typically part of the object tree being instantiated</param>
        /// <param name="constructorParameterSpecs"></param>
        /// <param name="constructorName"></param>
        /// <param name="injectionState"></param>
        /// <exception>InvalidArgumentException</exception>  
        private (object bean, InjectionState injectionState) Construct(Type beanType
            , IList<ChildBeanSpec> constructorParameterSpecs, string constructorName, InjectionState injectionState)
        {
            object[] args = new object[0];
            if (beanType.IsStruct())
            {
                return (Activator.CreateInstance(beanType), injectionState);
            }
            else  // class
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                ConstructorInfo constructorInfo  = constructorParameterSpecs?.Count > 0 
                  ? CheckForNull( () => beanType.GetConstructorNamed(constructorName))
                  : CheckForNull( () => beanType.GetNoArgConstructor(flags), new NoArgConstructorException(beanType.GetIOCCName()));
                if (constructorParameterSpecs?.Count > 0)
                {
                    InjectionState @is = injectionState;
                    var factoryConstructors = constructorParameterSpecs.Where(spec => spec.IsFactory).Select(
                        spec =>
                        {
                            object obj;
                            (obj, @is) = spec.ExecuteFactory(@is, new BeanFactoryArgs(
                                spec.ParameterInfo.GetBeanReferenceAttribute()
                                    .FactoryParameter));
                            return (obj, @is);
                        }).Select(p => p.obj);
                    args = factoryConstructors.Union(constructorParameterSpecs.Where(spec => !spec.IsFactory)
                        .Select(spec => spec.MemberOrFactoryBean)).ToArray();
                    args.Where(arg => arg != null).Select(arg =>
                    {
                        LogConstructorInjection(@is.Diagnostics, beanType, arg.GetType());
                        return arg;
                    }).ToList();
                   List<object> parameters = new List<object>();
                   foreach (var spec in constructorParameterSpecs)
                    {
                        if (spec.IsFactory)
                        {
                            object obj;
                            try
                            {
                                (obj, injectionState) = (spec.MemberOrFactoryBean as IFactory)
                                 .Execute(injectionState, new BeanFactoryArgs(
                                  spec.ParameterInfo.GetBeanReferenceAttribute()
                                    .FactoryParameter));                           
                            }
                            catch (Exception ex)
                            {
                                throw new DIException($"Execute failed for {spec.MemberOrFactoryBean.GetType().FullName}"
                                  ,ex, injectionState.Diagnostics);
                            }                            
                            parameters.Add(obj);
                            LogConstructorInjection(injectionState.Diagnostics, beanType, obj.GetType());
                            // TODO it would be good to catch type mismatches during eventual construction
                        }
                        else
                        {
                            parameters.Add(spec.MemberOrFactoryBean);
                            LogConstructorInjection(injectionState.Diagnostics, beanType, spec.MemberOrFactoryBean?.GetType());
                        }
                    }
                    args = parameters.ToArray();
                }
                try
                {
                    return (constructorInfo.Invoke(flags | BindingFlags.CreateInstance, null, args, null), injectionState);
                }
                catch (Exception ex2)
                {
                    throw new DIException($"Instantiation of {beanType.FullName} failed", ex2, injectionState.Diagnostics);
                }
            }        // construction of class
        }            // Construct

        private ConstructorInfo CheckForNull(Func<ConstructorInfo> func, Exception ex = null)
        {
            var ci = func();
            if (ci == null)
            {
                throw ex ?? new IOCCInternalException( "gone badly wrong");
                // TODO diagnostics and a good exception required here
            }

            return ci;
        }            

        private void LogConstructorInjection(Diagnostics diagnostics
          , Type declaringType, Type parameterImplementation)
        {
            Diagnostics.Group group = diagnostics.Groups["ConstructorInjectionsInfo"];
            dynamic diag = group.CreateDiagnostic();
            diag.DeclaringType = declaringType;
            diag.ParameterImplementation = parameterImplementation;
            group.Add(diag);
        }
    }                // ObjectTree
}
