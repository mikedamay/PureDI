using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Reflection;
using PureDI.Common;
using PureDI.Attributes;
using PureDI.Public;
using static PureDI.Common.Common;

namespace PureDI.Tree
{
    internal class ObjectTree
    {
        private delegate (bool constructionComplete, object beanId) BeanMaker(BeanScope beanScope, BeanSpec beanSpec
            , Type constructableType
            , IDictionary<InstantiatedBeanId, object> mapObjectsCreatedSoFar
            , Diagnostics diagnostics
            , IReadOnlyList<ChildBeanSpec> constructorParameterSpecs = null);
        private const BindingFlags constructorFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        private static readonly ClassScraper _classScraper = new ClassScraper();
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
                (rootObject, injectionState) = CreateObjectTree(new BeanSpec(rootType, rootBeanName, rootConstructorName)
                    ,injectionState.CreationContext, injectionState, new BeanReferenceDetails(), scope, MakeBean);
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
        public InjectionState CreateAndInjectDependencies(object rootObject,
            InjectionState injectionState, RootBeanSpec rootBeanSpec = null, bool deferDependencyInjection = false)
        {
            Type constructableType = rootObject.GetType();
            rootBeanSpec = rootBeanSpec ?? new RootBeanSpec();
            rootBeanSpec = rootBeanSpec.Scope == BeanScope.Prototype
              ? new RootBeanSpec("prototype-" + Guid.NewGuid().ToString()
              , rootBeanSpec.RootConstrutorName, rootBeanSpec.Scope)
              : rootBeanSpec;
                // in the call to CreateObjectTree we trick the method to create the dependencies
                // by adding the prototype uniquely to both the type map and the deferred assignment list
            var rodt = new RootObjectDecisionTable(deferDependencyInjection, rootBeanSpec.Scope);            
            string beanName = rootBeanSpec.RootBeanName;
            rodt.MaybeAddBeanToTypeMap(
              () => injectionState = UpdateTypeMap(injectionState, rootBeanSpec, constructableType, beanName));
            rodt.MaybeAddObjectToCreatedSoFarMap(
              () => injectionState.MapObjectsCreatedSoFar[new InstantiatedBeanId(constructableType
              ,beanName, rootBeanSpec.RootConstrutorName)] = rootObject);
            rodt.MaybeAddDeferredIn(
              () => injectionState.CreationContext.BeansWithDeferredAssignments
              .Add(new ConstructableBean(rootObject.GetType(), beanName)));
            BeanMaker rootBeanMaker = (bs, bs2, ctx, mocf, diags, cp) => { return (true, rootObject); };
            (_, injectionState) = CreateObjectTree(new BeanSpec(constructableType, beanName, rootBeanSpec.RootConstrutorName)
              ,injectionState.CreationContext, injectionState, new BeanReferenceDetails(), rootBeanSpec.Scope, rootBeanMaker);
            rodt.MaybeAddDeferredOut(
              () => injectionState.CreationContext.BeansWithDeferredAssignments
              .Add(new ConstructableBean(rootObject.GetType(), beanName)));
            return injectionState;
        }

        private InjectionState UpdateTypeMap(InjectionState injectionState, RootBeanSpec rootBeanSpec, Type constructableType,
            string beanName)
        {
            if (!injectionState.TypeMap.ContainsKey((constructableType, rootBeanSpec.RootBeanName)))
            {
                injectionState = AddRootObjectDetailsToTypeMap(injectionState, (constructableType, beanName));
            }
            else
            {
                if (injectionState.MapObjectsCreatedSoFar.ContainsKey(new InstantiatedBeanId(constructableType
                    , rootBeanSpec.RootBeanName, rootBeanSpec.RootConstrutorName)))
                {
                    RecordDiagnostic(injectionState.Diagnostics, "RootObjectExists"
                        , ("BeanType", constructableType.FullName));
                }
            }
            return injectionState;
        }

        /// <summary>
        /// see documentation for CreateAndInjectDependencies
        /// </summary>
        /// <param name="beanSpec">the type + beanName for which a bean is to be created.
        ///     The bean will not necessarily have the type passed in as this
        ///     may be a base class or interface (constructed generic type)
        ///     from which the bean is derived</param>
        /// <param name="creationContext"></param>
        /// <param name="injectionState"></param>
        /// <param name="declaringBeanDetails">provides a context to the bean that
        ///     can be displayed in diagnostic messages - currently not used for
        ///     anything else</param>
        /// <param name="beanScope"></param>
        /// <param name="beanMaker">normally MakeBean is used to instantiate the bean but
        ///   where a root object is passed in this method is by-passed by stubbing a simple
        ///   replacement that returns the root object</param>
        private (object bean, InjectionState injectionState) 
          CreateObjectTree(BeanSpec beanSpec, CreationContext creationContext
          ,InjectionState injectionState, BeanReferenceDetails declaringBeanDetails
          ,BeanScope beanScope, BeanMaker beanMaker)
        {
  
            CycleGuard cycleGuard = creationContext.CycleGuard;
            ISet<ConstructableBean> beansWithDeferredAssignments = creationContext.BeansWithDeferredAssignments;
            object bean;
            Type constructableType;
            if ((constructableType = MakeConstructableType(beanSpec, declaringBeanDetails
              ,injectionState.TypeMap, injectionState.Diagnostics)) == null)
            {
                return (null, injectionState);
            }
            bool cyclicalDependencyFound = false;
            var constructableBeanx = new ConstructableBean(constructableType, beanSpec.BeanName);
            try
            {
                var constructableBean = new ConstructableBean(constructableType, beanSpec.BeanName );
                List<ChildBeanSpec> beanSpecs = new List<ChildBeanSpec>();
                cyclicalDependencyFound = cycleGuard.IsPresent(constructableBean);
                if (!cyclicalDependencyFound)
                {
                    cycleGuard.Push(constructableBean);
                    var beanReferences =
                      _classScraper.GetMemberBeanReferences(constructableType, injectionState.Diagnostics)
                      .Concat(_classScraper.GetConstructorParameterBeanReferences(
                      constructableType, beanSpec.ConstructorName, injectionState.Diagnostics))
                      ;
                    foreach (ParamOrMemberInfo beanReference in beanReferences)
                    {
                        object memberBean = null;
                        var beanReferenceDetails = new BeanReferenceDetails(constructableType
                          ,beanReference.Name, beanReference.BeanName);
                        if (beanReference.IsFactory)
                        {
                            object oFactory = null;
                            (oFactory, injectionState) = CreateObjectTree(new BeanSpec(beanReference.Factory, beanReference.BeanName
                              ,beanReference.ConstructorName)
                              ,creationContext, injectionState, beanReferenceDetails, beanReference.Scope, MakeBean);
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
                                new BeanSpec(beanReference.Type, beanReference.BeanName, beanReference.ConstructorName)
                                ,creationContext, injectionState
                                ,beanReferenceDetails, beanReference.Scope, MakeBean);
                        } // not a factory

                        if (memberBean != null)
                        {
                            beanSpecs.Add(new ChildBeanSpec(
                                beanReference, memberBean, false));                            
                        }
                    }

                    bool complete;
                    (complete, bean) = beanMaker(beanScope, beanSpec, constructableType
                      ,injectionState.MapObjectsCreatedSoFar
                      ,injectionState.Diagnostics
                      ,beanSpecs.Where(bs => bs.Role == ChildBeanSpec.Roles.ConstructorParameter).ToList() );
                    Assert(!beansWithDeferredAssignments.Contains(constructableBeanx)
                           || beansWithDeferredAssignments.Contains(constructableBeanx)
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
                    if (complete && !beansWithDeferredAssignments.Contains(constructableBeanx))
                    {
                        return (bean, injectionState); // either the bean and therefore its children had already been created
                                     // or we were unable to create the bean (null)
                    }
                    beansWithDeferredAssignments.Remove(constructableBeanx);
                    AssignMembers(bean
                      ,beanSpecs.Where(bs => bs.Role == ChildBeanSpec.Roles.Member).ToList()
                      ,injectionState.Diagnostics);
                }
                else // there is a cyclical dependency so we
                    // need to just create the bean itself and defer child creation
                    // until the implementationType is encountered again
                    // further up the stack
                {
                    if (constructableType.HasInjectedConstructorParameters(beanSpec.ConstructorName))
                    {
                        dynamic diag = injectionState.Diagnostics.Groups["CyclicalDependency"].CreateDiagnostic();
                        diag.Bean = constructableType.FullName;
                        injectionState.Diagnostics.Groups["CyclicalDependency"].Add(diag);
                        throw new DIException("Cannot create this bean due to a cyclical dependency", injectionState.Diagnostics);
                    }
                    (_, bean) = beanMaker(beanScope, beanSpec, constructableType
                      ,injectionState.MapObjectsCreatedSoFar, injectionState.Diagnostics);
                    if (bean != null)
                    {
                        beansWithDeferredAssignments.Add(constructableBeanx);
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
          MakeBean(BeanScope beanScope, BeanSpec beanSpec
            ,Type constructableType
            ,IDictionary<InstantiatedBeanId, object> mapObjectsCreatedSoFar
            ,Diagnostics diagnostics
            ,IReadOnlyList<ChildBeanSpec> constructorParameterSpecs = null)
        {            
            object constructedBean;
            try
            {
                if (beanScope != BeanScope.Prototype
                    && mapObjectsCreatedSoFar.ContainsKey(
                    new InstantiatedBeanId(constructableType, beanSpec.BeanName, beanSpec.ConstructorName)))
                {
                    // there maybe a cyclical dependency
                    constructedBean = mapObjectsCreatedSoFar[
                      new InstantiatedBeanId(constructableType, beanSpec.BeanName, beanSpec.ConstructorName)];
                    return (true, constructedBean);
                }
                else
                {
                    // TODO explain why type to be constructed is complicated by generics
                    constructedBean = Construct(constructableType
                        , constructorParameterSpecs, beanSpec.ConstructorName, diagnostics);
                    if (beanScope != BeanScope.Prototype)
                    {
                        mapObjectsCreatedSoFar[
                          new InstantiatedBeanId(constructableType, beanSpec.BeanName, beanSpec.ConstructorName)] 
                          = constructedBean;
                    }
                }
            }
            catch (NoArgConstructorException inace)
            {
                RecordDiagnostic(diagnostics, "MissingNoArgConstructor"
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
        void AssignMembers(object declaringBean
            , IReadOnlyList<ChildBeanSpec> childrenArg, Diagnostics diagnostics)
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
                            RecordDiagnostic(diagnostics, "AlreadyInitialised"
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
                        RecordDiagnostic(diagnostics, "TypeMismatch"
                            , ("DeclaringBean", declaringBean.GetType().FullName)
                            , ("Member", memberSpec.FieldOrPropertyInfo.Name)
                            , ("Factory", memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().Factory
                                .FullName)
                            , ("ExpectedType", memberSpec.FieldOrPropertyInfo.MemberType)
                            , ("Exception", ae));
                    }

                    LogMemberInjection(diagnostics, declaringBean.GetType()
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
        }  // AssignMembers()

        Type MakeConstructableType(BeanSpec beanSpec
          ,BeanReferenceDetails declaringBeanDetails
          ,TypeMap typeMap, Diagnostics diagnostics)
        {
            Type implementationType;
            if ((implementationType
              = GetImplementationType(beanSpec, declaringBeanDetails
              ,typeMap, diagnostics)) == null)
            {
                Assert(diagnostics.HasWarnings);    // diagnostics recorded by callee.
                return null; // no implementation type found corresponding to this beanId
                // we can still carry on and a) this might not be fatal b) other diagnostics may show up
                            
            }

            return MakeConstructableType(beanSpec, implementationType);
        }

        Type MakeConstructableType(BeanSpec beanSpec,
            Type implementationType)
            => implementationType.IsGenericType
                ? MakeGenericConstructableType(beanSpec, implementationType)
                : implementationType;

        private static Type MakeGenericConstructableType(
          BeanSpec beanSpec
          ,Type implementationType)
        {
            Assert(implementationType.IsGenericType);
            Assert(beanSpec.Type.IsGenericType);
            return implementationType.MakeGenericType(beanSpec.Type.GetGenericArguments());
        }

        /*
          * finds the matching concrete type (bean) for some member reference
          * where the member reference might be a base class or interface together
          * with an optional bean name (held as part of the bean reference attribute
          * which allows the container to choose between multiple matching concrete classes
          * Alternatively the member reference may be the implementationType itself.
          */
        Type 
          GetImplementationType(BeanSpec beanSpec
          ,BeanReferenceDetails beanReferenceDetails
          ,TypeMap typeMap, Diagnostics diagnostics)
        {
            if ( IsBeanPresntInTypeMap(beanSpec, typeMap))
            {
                var implementationType = GetImplementationFromTypeMap(beanSpec, typeMap);
                return implementationType;
            }
            else    // error
            {
                if (beanReferenceDetails.IsRoot)
                {
                    RecordDiagnostic(diagnostics, "MissingRoot"
                        , ("BeanType", beanSpec.Type.GetSafeFullName())
                        , ("BeanName", beanSpec.BeanName)
                    );
                    throw new DIException("failed to create object tree - see diagnostics for detail",
                        diagnostics);
                }
                else
                {
                    RecordDiagnostic(diagnostics, "MissingBean"
                        , ("Bean", beanReferenceDetails.DeclaringType.GetSafeFullName())
                        , ("MemberType", beanSpec.Type.GetSafeFullName())
                        , ("MemberName", beanReferenceDetails.MemberName)
                        , ("MemberBeanName", beanReferenceDetails.MemberBeanName)
                    );
                    return null;
                }
            }
        }

        /// <param name="beanSpec">Typically this is the type of a member 
        ///     marked as a bean reference with [IOCCBeanReference]
        ///     for generics bean type is a generic type definition</param>
        /// <param name="typeMap"></param>
        /// <returns>This will be a concrete class marked as a bean with [Bean] which
        ///     is derived from the beanId.beanType.  For generics this will be a
        ///     constructed generic type</returns>
        private Type
          GetImplementationFromTypeMap(BeanSpec beanSpec
          ,TypeMap typeMap)
        {

            char[] a = beanSpec.Type.GetSafeFullName().TakeWhile(n => n != '[').ToArray();
            string beanTypeName = new String(a);
            // trim the generic arguments from a generic
            Type referenceType = typeMap.Keys.FirstOrDefault(
                k => k.beanType.GetSafeFullName() == beanTypeName && k.beanName == beanSpec.BeanName).beanType;
            return typeMap[(referenceType, beanSpec.BeanName)];

        }

        /// <param name="typeMap"></param>
        /// <param name="beanSpec"><see cref="GetImplementationFromTypeMap"/></param>
        private bool IsBeanPresntInTypeMap(BeanSpec beanSpec, TypeMap typeMap)
        {
            char[] a = beanSpec.Type.IsArray 
              ? beanSpec.Type.FullName.ToArray() 
              : beanSpec.Type.FullName.TakeWhile(n => n != '[').ToArray();
            string beanTypeName = new String(a);
            // trim the generic arguments from a generic
            return typeMap.Keys.Any(k => k.beanType.GetSafeFullName() 
              == beanTypeName && k.beanName == beanSpec.BeanName);
        }

        /// <summary>checks if the type to be instantiated has a valid constructor and if so constructs it</summary>
        /// <param name="beanType">a concrete clasws typically part of the object tree being instantiated</param>
        /// <param name="constructorParameterSpecsArg">the parameters passed into the beanType's constructor
        ///     identified by constructorName.  The constructor parameters in this list
        ///     are guaranteed not to include factories.  They will have already been
        ///     resolved into the target parametes by the caller</param>
        /// <param name="constructorName">The name of one of beanType's constructors</param>
        /// <param name="diagnostics"></param>
        /// <exception>InvalidArgumentException</exception>  
        private object Construct(Type beanType
            , IReadOnlyList<ChildBeanSpec> constructorParameterSpecsArg, string constructorName,
            Diagnostics diagnostics)
        {
            var constructorParameterSpecs = constructorParameterSpecsArg ?? new List<ChildBeanSpec>();
            if (beanType.IsStruct())
            {
                return Activator.CreateInstance(beanType);
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
                LogConstructorInjections(diagnostics, beanType, constructorParameters);
                try
                {
                    return constructorInfo.Invoke(flags | BindingFlags.CreateInstance
                      ,null,  constructorParameters, null);
                }
                catch (Exception ex2)
                {
                    throw new DIException($"Instantiation of {beanType.FullName} failed"
                      ,ex2, diagnostics);
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
        private static void RecordCreationDiagnostics(InjectionState injectionState
          ,object oFactory, Type constructableType
          ,ParamOrMemberInfo beanReference)
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
                = new Dictionary<(Type beanType, string beanName), Type>(injectionState.TypeMap.GetDictionary()
                    // coreapp2.0 allowed a IReadonlyDictionary to be passed as a param to the constructor
                    // standard2.0 could not handle it
                );
            typeMap.Add(beanId, beanId.type);
            return new InjectionState(injectionState.Diagnostics, typeMap
                , injectionState.MapObjectsCreatedSoFar, injectionState.Assemblies, injectionState.CreationContext);
        }        
    }                // ObjectTree
}
