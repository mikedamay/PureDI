using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using PureDI.Attributes;
using static PureDI.Common.Common;

namespace PureDI.Tree
{
    /// <summary>
    /// Analyses a class (using reflection) to return details of member variables and constructor parameters
    /// </summary>
    internal class ClassScraper
    {
        private const BindingFlags constructorFlags =
            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        /// <summary>
        /// The set of parameters returned by this routine is the complete bunch of
        /// parameters for the target constructor.  Their presence guarantees that
        /// they are valid bean references and that they relate to a non-duplicate
        /// well formed constructor.
        /// </summary>
        /// <param name="declaringBeanType">the type which is being scraped</param>
        /// <param name="constructorName">the name of the required constructor - typically ""</param>
        /// <param name="diagnostics">repo for all warnings</param>
        /// <returns>A list of specifications of the parameters of the reqquired constructor
        /// (empty list if none or a default constructor)
        /// </returns>
        public IReadOnlyList<ChildBeanSpec> GetConstructorParameterBeanReferences(
          Type declaringBeanType, string constructorName
          , Diagnostics diagnostics
          )
        {
            WarnOfConstructorsWithMissingAttribute(declaringBeanType, diagnostics);
            List<ChildBeanSpec> @params = new List<ChildBeanSpec>();
            @params = new List<ChildBeanSpec>();
            ValidateConstructors(declaringBeanType, constructorName, diagnostics);
            if (declaringBeanType.GetConstructors(
              BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
              .Length > 0)
            {
                var paramInfos = GetParametersForConstructorMatching(declaringBeanType, constructorName);
                if (paramInfos != null)
                {
                    foreach (var paramInfo in paramInfos)
                    {
                        var childBeanSpec = CreateTreeForMemberOrParameter(
                          new ParamOrMemberInfo(paramInfo), declaringBeanType, diagnostics);
                        if (childBeanSpec != null)
                        {
                            @params.Add(childBeanSpec);
                        }
                    } // for each constructor parameter
                }
            }
            return @params.AsReadOnly();
        } // CreateConstructorTrees()
        
        public IReadOnlyList<ChildBeanSpec> GetMemberBeanReferences(
          Type declaringBeanType, Diagnostics diagnostics)
        {
            List<ChildBeanSpec> members = new List<ChildBeanSpec>();
            var fieldOrPropertyInfos = declaringBeanType.GetMembers(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(f => f is FieldInfo || f is PropertyInfo);
            foreach (var fieldOrPropertyInfo in fieldOrPropertyInfos)
            {
                var childBeanSpec = CreateTreeForMemberOrParameter(new ParamOrMemberInfo(fieldOrPropertyInfo)
                  ,declaringBeanType, diagnostics);
                if (childBeanSpec != null)
                {
                    members.Add(childBeanSpec);                   
                }
            } // for each property or field

            return members.AsReadOnly();
        } // CreateMemberTrees()

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
        private static void ValidateConstructors(Type declaringBeanType
            , string constructorName, Diagnostics diagnostics)
        {
            ConstructorInfo[] constructors
              = declaringBeanType.GetConstructors(constructorFlags
              ).Where(co => co.GetCustomAttributes<ConstructorBaseAttribute>()
              .Any(ca => ca.Name == constructorName)).ToArray();
            if (constructors.Length == 0)
            {
                return;
            }
            ValidateMatchingConstructors(declaringBeanType, diagnostics, constructors);
        }
        /// <summary>
        /// A constructor is considered for this warning if it contains parameters with BeanReference attributes
        /// but is not itself annotated as a [Constructor]
        /// </summary>
        /// <param name="declaringBeanType">the type whose constructors are examined</param>
        /// <param name="diagnostics">repository for warnings</param>
        private static void WarnOfConstructorsWithMissingAttribute(Type declaringBeanType, Diagnostics diagnostics)
        {
            if (declaringBeanType.GetConstructors().Where(
                    co => !co.GetCustomAttributes<ConstructorBaseAttribute>().Any())
                .Any(co => co.GetParameters().Any(
                    p => p.GetCustomAttributes<BeanReferenceBaseAttribute>().Any())))
            {
                dynamic diag = diagnostics.Groups["MissingConstructorAttribute"].CreateDiagnostic();
                diag.Bean = declaringBeanType;
                diagnostics.Groups["MissingConstructorAttribute"].Add(diag);
            }
        }

        private static void ValidateMatchingConstructors(Type declaringBeanType, Diagnostics diagnostics,
            ConstructorInfo[] constructors)
        {
            if (constructors.Length > 1)
            {
                dynamic diag = diagnostics.Groups["DuplicateConstructors"].CreateDiagnostic();
                diag.Bean = declaringBeanType;
                diagnostics.Groups["DuplicateConstructors"].Add(diag);
                throw new DIException(
                    $"{declaringBeanType} has duplicate constructors - please see diagnostics for further details"
                    , diagnostics);
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
                    throw new DIException(
                        $"{declaringBeanType}'s constructor has parameters not marked as [IOCCBeanReference] - please see diagnostics for further details",
                        diagnostics);
                }
            }
        }
        // null return indicates this member is not a valid bean reference or constructor parameter
        private static ChildBeanSpec CreateTreeForMemberOrParameter(ParamOrMemberInfo fieldOrPropertyInfo,
            Type declaringBeanType
            , Diagnostics diagnostics)
        {
            BeanReferenceBaseAttribute attr;
            ChildBeanSpec childBeanSpec = null;
            if ((attr = fieldOrPropertyInfo.GetCustomeAttribute<BeanReferenceBaseAttribute>()) != null)
            {
                (Type type, string beanName, string constructorName) memberBeanId =
                    MakeMemberBeanId(fieldOrPropertyInfo.Type
                        , attr.Name, attr.ConstructorName);
                //object memberBean;
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
                        object o = null;
                        if (!typeof(IFactory).IsAssignableFrom(attr.Factory))
                        {
                            RecordDiagnostic(diagnostics, "BadFactory"
                                , ("DeclaringBean", declaringBeanType.FullName)
                                , ("Member", fieldOrPropertyInfo.Name)
                                , ("Factory", attr.Factory.FullName)
                            );
                           
                        }
/*
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
*/
                        else // factory successfully created
                        {
                            IFactory factoryBean = (o as IFactory);
                            childBeanSpec = new ChildBeanSpec(fieldOrPropertyInfo, factoryBean, true);
                        }
                    }
                    else // create the member without using a factory
                    {
                        //memberBean = null;
/*
                        (memberBean, injectionState) = CreateObjectTree(memberBeanId, creationContext, injectionState
                          ,new BeanReferenceDetails(declaringBeanType
                          ,fieldOrPropertyInfo.Name, memberBeanId.beanName), attr.Scope);
*/
                        childBeanSpec = new ChildBeanSpec(fieldOrPropertyInfo, null, false);
                    } // not a factory
                } // writeable member
            } // this is a bean reference

            return childBeanSpec;
        }
        private static ParameterInfo[] GetParametersForConstructorMatching(
            Type declaringBeanType, string constructorName)
            => declaringBeanType
                .GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).FirstOrDefault(co
                    => co.GetCustomAttribute<ConstructorBaseAttribute>() != null
                       && string.Compare(co.GetCustomAttribute<ConstructorBaseAttribute>()?
                           .Name, constructorName, StringComparison.OrdinalIgnoreCase) == 0)?.GetParameters();
        
        private static void RecordDiagnostic(Diagnostics diagnostics, string groupName
          ,params (string member, object value)[] occurrences)
        {
            dynamic diag = diagnostics.Groups[groupName].CreateDiagnostic();
            foreach ((var member, var value) in occurrences)
            {
                diag.Members[member] = value;
            }
            diagnostics.Groups[groupName].Add(diag);
        }
        /// <summary>
        /// well, this is tricky.
        /// </summary>
        /// <param name="memberDeclaredBeanType"></param>
        /// <param name="memberDeclaredBeanName"></param>
        /// <param name="constructorName"></param>
        /// <returns></returns>
        private static (Type type, string beanName, string constructorName)
            MakeMemberBeanId(Type memberDeclaredBeanType
                , string memberDeclaredBeanName, string constructorName
            )
        {
            Assert(!memberDeclaredBeanType.IsGenericParameter);
            return (memberDeclaredBeanType, memberDeclaredBeanName, constructorName);
        }

    }
}