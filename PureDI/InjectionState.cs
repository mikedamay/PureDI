using System;
using System.Collections.Generic;
using System.Linq;
using PureDI.Common;
using System.Reflection;
using PureDI.Public;
using PureDI.Tree;

namespace PureDI
{
    /// <summary>
    /// This class contains the history of dependencies injected in
    /// previous calls to <codeInline>CreateAndInjectDependencies</codeInline>.
    /// It is returned by <codeInline>CreateAndInjectDependencies</codeInline> and the expectation
    /// is that it will be passed to subsequent calls.  This pattern allows 
    /// multiple calls to share the same data whilst supporting thread safety and
    /// avoiding memory leaks where injections are transitory
    /// </summary>
    public class InjectionState
    {
        private readonly Diagnostics diagnostics;
        private readonly TypeMap typeMap;
        private readonly IDictionary<InstantiatedBeanId, object> mapObjectsCreatedSoFar;
        private readonly Assembly[] _assemblies;
        private readonly CreationContext _creationContext;
        internal InjectionState(Diagnostics diagnostics
            , IReadOnlyDictionary<(Type beanType, string beanName), Type> typeMap
            , IDictionary<InstantiatedBeanId, object> mapObjectsCreatedSoFar, Assembly[] assemblies,
            CreationContext creationContext = null)
        {
            this.diagnostics = diagnostics;
            this.mapObjectsCreatedSoFar = mapObjectsCreatedSoFar;
            this.typeMap = new TypeMapImpl(typeMap);
            this._assemblies = assemblies;
            this._creationContext = creationContext;
        }

        internal void Deconstruct(out Diagnostics diagnostics
            , out IReadOnlyDictionary<(Type beanType, string beanName), Type> typeMap
            , out IDictionary<InstantiatedBeanId, object> mapObjectsCreatedSoFar
            ,out Assembly[] assemblies)
        {
            diagnostics = this.diagnostics;
            mapObjectsCreatedSoFar = this.mapObjectsCreatedSoFar;
            typeMap = this.typeMap;
            assemblies = _assemblies;

        }

        internal bool IsEmpty() => !diagnostics.HasWarnings && mapObjectsCreatedSoFar.Count == 0 && typeMap.Count == 0;
        internal InjectionState Clone()
        {
            return new InjectionState(
              new DiagnosticBuilder(this.Diagnostics).Diagnostics
              , new Dictionary<(Type beanType, string beanName), Type>(this.typeMap)
              , new Dictionary<InstantiatedBeanId, object>(mapObjectsCreatedSoFar)
              ,(Assembly[])_assemblies.Clone()
              );
        }
        /// <summary>
        /// sophisticated library users may want to analyze diagnostics programmatically.
        /// Access to the diagnostics supports this endeavor.
        /// </summary>
        public Diagnostics Diagnostics => diagnostics;

        // the key in the objects created so far map comprises 2 types.  The first is the
        // intended concrete type that will be instantiated.  This works well for
        // non-generic types but for generics the concrete type, which is taken from the typeMap,
        // is a generic type definition.  The builder needs to lay its hands on the type argument
        // to substitute for the generic parameter.  The second type (beanReferenceType) which
        // has been taken from the member information of the declaring task provides the generic argument
        internal TypeMap TypeMap => typeMap;
//        internal IReadOnlyDictionary<(Type beanType, string beanName), Type> TypeMap => typeMap;
        internal IDictionary<InstantiatedBeanId, object> MapObjectsCreatedSoFar => mapObjectsCreatedSoFar;
        internal Assembly[] Assemblies => _assemblies;
        internal CreationContext CreationContext => _creationContext;
        /// <summary>
        /// shortcut to diagnostics.AllToString().
        /// a multi-line string containing all warnings and other info.
        /// <see cref="PureDI.Diagnostics.AllToString"/>
        /// </summary>
        public string AllDiagnosticsToString() => diagnostics.AllToString();
        /// <summary>
        /// shortcut to diagnostics.ToString().
        /// multi-line string containing all warnings.
        /// <see cref="PureDI.Diagnostics.ToString"/>
        /// </summary>
        public string WarningsToString() => diagnostics.ToString();
        /// <summary>
        /// convenient starting point and useful for library user edge
        /// case where they want to call the container multiple times
        /// with no history.
        /// </summary>
        public static InjectionState Empty
          => new InjectionState(
                    new DiagnosticBuilder().Diagnostics
                    ,new Dictionary<(Type beanType, string beanName), Type>()
                    ,new Dictionary<InstantiatedBeanId, object>()
                    ,new Assembly[0], null);
    }
}