using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using PureDI.Common;
using PureDI.Attributes;
using static PureDI.Common.Common;

namespace PureDI.Tree
{
    internal class ObjectTree
    {
        private const BindingFlags constructorFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        private static ClassScraper _classScraper = new ClassScraper();
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
        /// <returns>an ojbect of root type</returns>
        public (object bean, InjectionState injectionState)
          CreateAndInjectDependencies(Type rootType, InjectionState injectionState, string rootBeanName
          ,string rootConstructorName, BeanScope scope)
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
            catch (NoArgConstructorException inace)    // I suspect this is never executed
            {
                dynamic diagnostic = injectionState.Diagnostics.Groups["MissingNoArgConstructor"].CreateDiagnostic();
                diagnostic.Class = rootType.GetSafeFullName();
                injectionState.Diagnostics.Groups["MissingNoArgConstructor"].Add(diagnostic);
                throw new DIException("Failed to create object tree - see diagnostics for details", inace,
                    injectionState.Diagnostics);
            }
        }
        public InjectionState CreateAndInjectDependencies(object rootObject, InjectionState injectionState)
        {
            Type constructableType = rootObject.GetType();
            string beanName;
            if (!injectionState.TypeMap.ContainsKey((constructableType, Constants.DefaultBeanName)))
            {
                beanName = Guid.NewGuid().ToString();
                injectionState = AddRootObjectDetailsToTypeMap(injectionState, (constructableType, beanName));                
            }
            else
            {
                beanName = Constants.DefaultBeanName;
            }
            injectionState.MapObjectsCreatedSoFar[new InstantiatedBeanId(constructableType
                ,beanName, Constants.DefaultConstructorName)] = rootObject;
            (_, injectionState) = CreateObjectTree((constructableType, beanName, Constants.DefaultConstructorName)
              ,new CreationContext(new CycleGuard()
              ,new HashSet<Type>{rootObject.GetType()}), injectionState, new BeanReferenceDetails(), BeanScope.Singleton);
            return injectionState;
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
        /// <param name="declaringBeanDetails">provides a context to the bean that
        ///     can be displayed in diagnostic messages - currently not used for
        ///     anything else</param>
        /// <param name="beanScope"></param>
        private (object bean, InjectionState injectionState) 
          CreateObjectTree((Type beanType, string beanName
          ,string constructorName) beanId, CreationContext creationContext
          ,InjectionState injectionState, BeanReferenceDetails declaringBeanDetails
          ,BeanScope beanScope)
        {
            Type implementationType;
  
            CycleGuard cycleGuard = creationContext.CycleGuard;
            ISet<Type> beansWithDeferredAssignments = creationContext.BeansWithDeferredAssignments;
            bool complete;
            object bean;
            if (((implementationType, injectionState) 
              = GetImplementationType(beanId, declaringBeanDetails, injectionState)).implementationType == null)
            {
                Assert(injectionState.Diagnostics.HasWarnings);    // diagnostics recorded by callee.
                return (null, injectionState); // no implementation type found corresponding to this beanId
                             // we can still carry on and a) this might not be fatal b) other diagnostics may show up
                            
            }
            Type constructableType = MakeConstructableType(beanId, implementationType);
            bool cyclicalDependencyFound = false;
            try
            {
                List<ChildBeanSpec> beanSpecs = new List<ChildBeanSpec>();
                cyclicalDependencyFound = cycleGuard.IsPresent(constructableType);
                if (!cyclicalDependencyFound)
                {
                    cycleGuard.Push(constructableType);
                    var beanReferences =
                      _classScraper.GetMemberBeanReferences(constructableType, injectionState.Diagnostics)
                      .Concat(_classScraper.GetConstructorParameterBeanReferences(
                      constructableType, beanId.constructorName, injectionState.Diagnostics))
                      ;
                    foreach (ParamOrMemberInfo beanReference in beanReferences)
                    {
                        object memberBean = null;
                        var beanReferenceDetails = new BeanReferenceDetails(constructableType
                          ,beanReference.Name, beanReference.BeanName);
                        if (beanReference.IsFactory)
                        {
                            object oFactory = null;
                            (oFactory, injectionState) = CreateObjectTree((beanReference.Factory, beanReference.BeanName
                              ,beanReference.ConstructorName)
                              ,creationContext, injectionState, beanReferenceDetails, beanReference.Scope);
                            if (oFactory != null)
                            {
                                (memberBean, injectionState) = ExecuteFactory( injectionState, oFactory
                                  ,constructableType, beanReference);
                            }
                            RecordCreationDiagnostics(injectionState, oFactory, constructableType, beanReference);
                            
                        }
                        else // create the member without using a factory
                        {
                            (memberBean, injectionState) = CreateObjectTree(
                                (beanReference.Type, beanReference.BeanName, beanReference.ConstructorName)
                                ,creationContext, injectionState
                                ,beanReferenceDetails, beanReference.Scope);
                        } // not a factory

                        if (memberBean != null)
                        {
                            beanSpecs.Add(new ChildBeanSpec(
                                beanReference, memberBean, false));                            
                        }
                    }

                    (complete, bean) = MakeBean(beanScope, beanId, implementationType, injectionState
                      ,beanSpecs.Where(bs => bs.Role == ChildBeanSpec.Roles.ConstructorParameter).ToList() );
                    Assert(!beansWithDeferredAssignments.Contains(constructableType)
                           || beansWithDeferredAssignments.Contains(constructableType)
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
                    if (complete && !beansWithDeferredAssignments.Contains(constructableType))
                    {
                        return (bean, injectionState); // either the bean and therefore its children had already been created
                                     // or we were unable to create the bean (null)
                    }
                    beansWithDeferredAssignments.Remove(constructableType);
                    injectionState = AssignMembers(bean
                      ,beanSpecs.Where(bs => bs.Role == ChildBeanSpec.Roles.Member).ToList()
                      ,injectionState, creationContext);
                }
                else // there is a cyclical dependency so we
                    // need to just create the bean itself and defer child creation
                    // until the implementationType is encountered again
                    // further up the stack
                {
                    if (constructableType.HasInjectedConstructorParameters(beanId.constructorName))
                    {
                        dynamic diag = injectionState.Diagnostics.Groups["CyclicalDependency"].CreateDiagnostic();
                        diag.Bean = constructableType.FullName;
                        injectionState.Diagnostics.Groups["CyclicalDependency"].Add(diag);
                        throw new DIException("Cannot create this bean due to a cyclical dependency", injectionState.Diagnostics);
                    }
                    (_, bean) = MakeBean(beanScope, beanId, implementationType, injectionState);
                    if (bean != null)
                    {
                        beansWithDeferredAssignments.Add(constructableType);
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
        }
        
        (bool constructionComplete, object beanId) 
          MakeBean(BeanScope beanScope, (Type beanType, string beanName, string constructorName) beanIdArg, Type implementationType
            ,InjectionState injectionState,
            IReadOnlyList<ChildBeanSpec> constructorParameterSpecs = null)
        {            
            object constructedBean;
            try
            {
                Type constructableTypeLocal = MakeConstructableType(beanIdArg
                  ,implementationType);
                if (beanScope != BeanScope.Prototype
                    && injectionState.MapObjectsCreatedSoFar.ContainsKey(
                    new InstantiatedBeanId(constructableTypeLocal, beanIdArg.beanName, beanIdArg.constructorName)))
                {
                    // there maybe a cyclical dependency
                    constructedBean = injectionState.MapObjectsCreatedSoFar[
                      new InstantiatedBeanId(constructableTypeLocal, beanIdArg.beanName, beanIdArg.constructorName)];
                    return (true, constructedBean);
                }
                else
                {
                    // TODO explain why type to be constructed is complicated by generics
                    (constructedBean, injectionState) = Construct(constructableTypeLocal
                        , constructorParameterSpecs, beanIdArg.constructorName, injectionState);
                    if (beanScope != BeanScope.Prototype)
                    {
                        injectionState.MapObjectsCreatedSoFar[
                          new InstantiatedBeanId(constructableTypeLocal, beanIdArg.beanName, beanIdArg.constructorName)] 
                          = constructedBean;
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

        private static (object, InjectionState) ExecuteFactory(InjectionState injectionState
            , object oFactory
            , Type constructableType, ParamOrMemberInfo beanReference)
        {
            try
            {
                return (oFactory as IFactory).Execute(injectionState
                    , new BeanFactoryArgs(beanReference.FactoryParameter));
            }
            catch (Exception ex)
            {
                RecordDiagnostic(injectionState.Diagnostics, "FactoryExecutionFailure"
                    , ("DeclaringBean", constructableType.FullName)
                    , ("Member", beanReference.Name)
                    , ("Factory", beanReference.Factory.FullName)
                    , ("Exception", ex));
                throw new DIException("factory execution failed", ex, injectionState.Diagnostics);
            }
        }

        // declaringBean - the bean just returned by MakeBean()
        InjectionState AssignMembers(object declaringBean, IReadOnlyList<ChildBeanSpec> childrenArg, InjectionState injectionState, CreationContext creationContext)
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

                    try
                    {
                        memberSpec.FieldOrPropertyInfo.SetValue(declaringBean, memberBean);

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

                    LogMemberInjection(injectionState.Diagnostics, declaringBean.GetType()
                        , memberSpec.FieldOrPropertyInfo.GetDeclaredType()
                        , memberSpec.FieldOrPropertyInfo.Name
                        , memberBean.GetType());
                }
            }
            foreach (var memberSpec in childrenArg)
            {
                object memberBean = default;
                memberBean = memberSpec.MemberOrFactoryBean;
                AssignBean(memberSpec, memberBean);
            }       // foreach memberSpec
            return injectionState;
        }  // AssignMembers()


        Type MakeConstructableType((Type beanType, string beanName, string constructorName) beanIdArg,
            Type implementationTypeArg)
            => implementationTypeArg.IsGenericType
                ? MakeGenericConstructableType(beanIdArg, implementationTypeArg)
//                ? beanIdArg.beanType
                : implementationTypeArg;

        private static Type MakeGenericConstructableType(
          (Type beanType, string beanName, string constructorName) beanIdArg
          ,Type implementationTypeArg)
        {
            return implementationTypeArg.MakeGenericType(beanIdArg.beanType.GetGenericArguments());
        }

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
            else    // error
            {
                injectionState = result.injectionState;
                if (beanReferenceDetails.IsRoot)
                {
                    RecordDiagnostic(injectionState.Diagnostics, "MissingRoot"
                        , ("BeanType", beanId.Item1.GetSafeFullName())
                        , ("BeanName", beanId.Item2)
                    );
                    throw new DIException("failed to create object tree - see diagnostics for detail",
                        injectionState.Diagnostics);
                }
                else
                {
                    RecordDiagnostic(injectionState.Diagnostics, "MissingBean"
                        , ("Bean", beanReferenceDetails.DeclaringType.GetSafeFullName())
                        , ("MemberType", beanId.Item1.GetSafeFullName())
                        , ("MemberName", beanReferenceDetails.MemberName)
                        , ("MemberBeanName", beanReferenceDetails.MemberBeanName)
                    );
                    return (null, injectionState);
                }
            }
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

            char[] a = beanId.beanType.GetSafeFullName().TakeWhile(n => n != '[').ToArray();
            string beanTypeName = new String(a);
            // trim the generic arguments from a generic
            Type referenceType = injectionState.TypeMap.Keys.FirstOrDefault(
                k => k.beanType.GetSafeFullName() == beanTypeName && k.beanName == beanId.beanName).beanType;
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
            return (injectionState.TypeMap.Keys.Any(k => k.beanType.GetSafeFullName() 
              == beanTypeName && k.beanName == beanId.beanName), injectionState);
        }

        /// <summary>checks if the type to be instantiated has a valid constructor and if so constructs it</summary>
        /// <param name="beanType">a concrete clasws typically part of the object tree being instantiated</param>
        /// <param name="constructorParameterSpecsArg">the parameters passed into the beanType's constructor
        ///        identified by constructorName.  The constructor parameters in this list
        ///        are guaranteed not to include factories.  They will have already been
        ///        resolved into the target parametes by the caller</param>
        /// <param name="constructorName">The name of one of beanType's constructors</param>
        /// <param name="injectionState"></param>
        /// <exception>InvalidArgumentException</exception>  
        private (object bean, InjectionState injectionState) Construct(Type beanType
            , IReadOnlyList<ChildBeanSpec> constructorParameterSpecsArg, string constructorName, InjectionState injectionState)
        {
            var constructorParameterSpecs = constructorParameterSpecsArg ?? new List<ChildBeanSpec>();
            InjectionState @is = injectionState;
            if (beanType.IsStruct())
            {
                return (Activator.CreateInstance(beanType), injectionState);
            }
            else  // class
            {
                BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
                ConstructorInfo constructorInfo  = constructorParameterSpecs?.Count > 0 
                  ? Check( () => beanType.GetConstructorNamed(constructorName))
                  : Check( () => beanType.GetNoArgConstructor(flags), new NoArgConstructorException(beanType.GetSafeFullName()));
                        // it is obvious that the system has already determined the correct constructor
                        // by dint of the fact that the appropriate parameters are passed in (and it would
                        // need the constructorInfo to enumerate those).  Why is the name rather than constructorInfo passed in?
                        // The reason is that when Construct is called for a bean where a specified constructor
                        // is not referenced then there is no requirement by the caller to identify a constructor.
                        // The downside is that the link is temporarily lost between the tightly coupled
                        // consructorInfo and the parameters.
                var constructorParameters = new object[0].Concat(constructorParameterSpecs.Where(spec => !spec.IsFactory)
                    .Select(spec => spec.MemberOrFactoryBean)).ToArray();
                LogConstructorInjections(@is.Diagnostics, beanType, constructorParameters);
                try
                {
                    return (constructorInfo.Invoke(flags | BindingFlags.CreateInstance
                      ,null,  constructorParameters, null), @is);
                }
                catch (Exception ex2)
                {
                    throw new DIException($"Instantiation of {beanType.FullName} failed"
                      ,ex2, injectionState.Diagnostics);
                }
            }        // construction of class
        }            // Construct

        private ConstructorInfo Check(Func<ConstructorInfo> func, Exception ex = null)
        {
            var ci = func();
            if (ci == null)
            {
                throw ex ?? new IOCCInternalException( "gone badly wrong");
                // TODO diagnostics and a good exception required here
            }

            return ci;
        }

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

        private void LogConstructorInjections(Diagnostics diagnostics
            , Type declaringType, object[] args)
        {
            void LogConstructorInjection(
                Type parameterImplementation)
            {
                Diagnostics.Group group = diagnostics.Groups["ConstructorInjectionsInfo"];
                dynamic diag = group.CreateDiagnostic();
                diag.DeclaringType = declaringType;
                diag.ParameterImplementation = parameterImplementation;
                group.Add(diag);
            }
            args.Where(arg => arg != null).Select(arg =>
            {
                LogConstructorInjection(arg.GetType());
                return arg;
            }).ToList();
        }
        private static void RecordCreationDiagnostics(InjectionState injectionState, object oFactory, Type constructableType,
            ParamOrMemberInfo beanReference)
        {
            if (oFactory == null)
            {
                RecordDiagnostic(injectionState.Diagnostics, "MissingFactory"
                    , ("DeclaringBean", constructableType.FullName)
                    , ("Member", beanReference.FieldOrPropertyInfo.Name)
                    , ("Factory", beanReference.Factory.FullName)
                    , ("ExpectedType", new ParamOrMemberInfo(beanReference.FieldOrPropertyInfo).Type));
            }
            else if (!(oFactory is IFactory))
            {
                RecordDiagnostic(injectionState.Diagnostics, "BadFactory"
                    , ("DeclaringBean", constructableType.FullName)
                    , ("Member", beanReference.FieldOrPropertyInfo.Name)
                    , ("Factory", beanReference.Factory.FullName)
                );
            }
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
        private InjectionState AddRootObjectDetailsToTypeMap(
            InjectionState injectionState, (Type type, string beanName) beanId)
        {
            Dictionary<(Type beanType, string beanName), Type> typeMap
                = new Dictionary<(Type beanType, string beanName), Type>(injectionState.TypeMap);
            typeMap.Add(beanId, beanId.type);
            return new InjectionState(injectionState.Diagnostics, typeMap
                , injectionState.MapObjectsCreatedSoFar, injectionState.Assemblies);
        }
        
    }                // ObjectTree
}
