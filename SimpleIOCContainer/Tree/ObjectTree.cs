using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static com.TheDisappointedProgrammer.IOCC.Common;

namespace com.TheDisappointedProgrammer.IOCC.Tree
{
    internal class ObjectTree
    {
        public static int s_nAssignments;
        private readonly string profile;

        private const BindingFlags constructorFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        private readonly IDictionary<(Type type, string beanName), Type> typeMap;
        // from the point of view of generics the key.type may contain a generic type definition
        // and the value may be a constructed generic type

        public ObjectTree(string profile
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
        public object CreateAndInjectDependencies(Type rootType
            , IOCCDiagnostics diagnostics
            , string rootBeanName, string rootConstructorName, BeanScope scope,
            IDictionary<(Type, string), object> mapObjectsCreatedSoFar)
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
            Console.WriteLine($"number of assignments: {s_nAssignments}");
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
        /// see documentation for CreateAndInjectDependencies
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
        /// <param name="bean">a class already instantiated by SimpleIOCContainer whose
        ///                    fields and properties may need to be injuected</param>
        private object CreateObjectTree((Type beanType, string beanName, string constructorName) beanId,
            CreationContext creationContext, IOCCDiagnostics diagnostics, BeanReferenceDetails beanReferenceDetails,
            BeanScope beanScope)
        {
             Type implementationType;

            (bool constructionComplete, object beanId) MakeBean(IList<ChildBeanSpec> constructorParameterSpecs = null)
            {            
                object constructedBean;
                try
                {
                    Type constructableTypeLocal = MakeConstructableType(beanId, implementationType);
                    if (beanScope != BeanScope.Prototype
                        && creationContext.MapObjectsCreatedSoFar.ContainsKey((constructableTypeLocal, beanId.constructorName)))
                    {
                        // there maybe a cyclical dependency
                        constructedBean = creationContext.MapObjectsCreatedSoFar[(constructableTypeLocal, beanId.constructorName)];
                        return (true, constructedBean);
                    }
                    else
                    {
                        // TODO explain why type to be constructed is complicated by generics
                        constructedBean = Construct(constructableTypeLocal
                            , constructorParameterSpecs, beanId.constructorName);
                        if (beanScope != BeanScope.Prototype)
                        {
                            creationContext.MapObjectsCreatedSoFar[(constructableTypeLocal, beanId.constructorName)] = constructedBean;
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
                BeanReferenceAttribute attr;
                if ((attr = fieldOrPropertyInfo.GetCustomeAttribute<BeanReferenceAttribute>()) != null)
                {
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
                            else if (!(o is IFactory))
                            {
                                RecordDiagnostic(diagnostics, "BadFactory"
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
                ValidateConstructors(declaringBeanType, constructorName, diagnostics, beanReferenceDetails);
                if (declaringBeanType.GetConstructors(
                  BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                  .Length > 0)
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
                void AssignBean(ChildBeanSpec memberSpec, object memberBean)
                {
                    if (memberBean != null)
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
                        s_nAssignments++;
                        memberSpec.FieldOrPropertyInfo.SetValue(declaringBean, memberBean);
                    }
                }
                foreach (var memberSpec in childrenArg)
                {
                    object memberBean = null;
                    if (memberSpec.IsFactory)
                    {
                        try
                        {
                            IFactory factory = memberSpec.MemberOrFactoryBean as IFactory;
                            memberBean = factory.Execute(new BeanFactoryArgs(
                                memberSpec.FieldOrPropertyInfo.GetBeanReferenceAttribute().FactoryParameter));
                            CreateMemberTrees(memberBean.GetType(), out var memberBeanMembers);
                            AssignMembers(memberBean, memberBeanMembers);
                            AssignBean(memberSpec, memberBean);
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
                    else    // non-factory
                    {
                        memberBean = memberSpec.MemberOrFactoryBean;
                        AssignBean(memberSpec, memberBean);
                    }
                }       // foreach memberSpec
            }           // AssignMembers()


            CycleGuard cycleGuard = creationContext.CycleGuard;
            ISet<Type> cyclicalDependencies = creationContext.CyclicalDependencies;
            bool complete;
            object bean;
            if ((implementationType = GetImplementationType(beanId, beanReferenceDetails, diagnostics)) == null)
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
                    if (constructableType.HasInjectedConstructorParameters(SimpleIOCContainer.DEFAULT_CONSTRUCTOR_NAME))
                    {
                        dynamic diag = diagnostics.Groups["CyclicalDependency"].CreateDiagnostic();
                        diag.Bean = constructableType.FullName;
                        diagnostics.Groups["CyclicalDependency"].Add(diag);
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
          ,string constructorName, IOCCDiagnostics diagnostics
          ,BeanReferenceDetails beanReferenceDetails)
        {
            ConstructorInfo[] constructors
              = declaringBeanType.GetConstructors(constructorFlags
              ).Where(co => co.GetCustomAttributes<ConstructorAttribute>()
              .Any(ca => ca.Name == constructorName)).ToArray();
            if (declaringBeanType.GetConstructors().Where(
                    co => !co.GetCustomAttributes<ConstructorAttribute>().Any())
                .Any(co => co.GetParameters().Any(
                    p => p.GetCustomAttributes<BeanReferenceAttribute>().Any())))
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
                throw new IOCCException($"{declaringBeanType} has duplicate constructors - please see diagnostics for further details",diagnostics);
            }
            if (constructors.Length > 0)
            {
                ConstructorInfo constructor = constructors[0];
                if (constructor.GetParameters().Length > 0
                    && !constructor.GetParameters().All(p => p.GetCustomAttributes<BeanReferenceAttribute>().Any()))
                {
                    dynamic diag = diagnostics.Groups["MissingConstructorParameterAttribute"].CreateDiagnostic();
                    diag.Bean = declaringBeanType;
                    diagnostics.Groups["MissingConstructorParameterAttribute"].Add(diag);
                    throw new IOCCException($"{declaringBeanType}'s constructor has parameters not marked as [IOCCBeanReference] - please see diagnostics for further details", diagnostics);
                }
            }
        }

        ParameterInfo[] GetParametersForConstructorMatching(
            Type declaringBeanTypeArg, string constructorNameArg)
            => declaringBeanTypeArg
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).FirstOrDefault(co
                    => co.GetCustomAttribute<ConstructorAttribute>() != null
                       && co.GetCustomAttribute<ConstructorAttribute>()?
                           .Name == constructorNameArg)?.GetParameters();
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
        Type GetImplementationType((Type, string, string) beanId, BeanReferenceDetails beanReferenceDetails
          ,IOCCDiagnostics diagnostics)
        {
            if (IsBeanPresntInTypeMap(beanId))
            {
                Type implementationType = GetImplementationFromTypeMap(beanId);
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
        /// <returns>This will be a concrete class marked as a bean with [Bean] which
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
            char[] a = beanId.beanType.IsArray 
              ? beanId.beanType.FullName.ToArray() 
              : beanId.beanType.FullName.TakeWhile(n => n != '[').ToArray();
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
                            
                            object obj = (spec.MemberOrFactoryBean as IFactory)
                              .Execute(new BeanFactoryArgs(
                              spec.ParameterInfo.GetBeanReferenceAttribute()
                              .FactoryParameter));
                            parameters.Add(obj);
                            // TODO it would be good to catch type mismatches during eventual construction
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
    }
}
