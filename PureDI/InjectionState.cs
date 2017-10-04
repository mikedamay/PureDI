﻿using System;
using System.Collections.Generic;
using System.Linq;
using PureDI.Common;

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
        private readonly IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap;
        private readonly IDictionary<(Type, string), object> mapObjectsCreatedSoFar;
        internal InjectionState(Diagnostics diagnostics
          ,IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap
          ,IDictionary<(Type, string), object> mapObjectsCreatedSoFar)
        {
            this.diagnostics = diagnostics;
            this.mapObjectsCreatedSoFar = mapObjectsCreatedSoFar;
            this.typeMap = typeMap;
        }

        internal void Deconstruct(out Diagnostics diagnostics
            , out IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap
            , out IDictionary<(Type, string), object> mapObjectsCreatedSoFar)
        {
            diagnostics = this.diagnostics;
            mapObjectsCreatedSoFar = this.mapObjectsCreatedSoFar;
            typeMap = this.typeMap;

        }

        internal bool IsEmpty() => !diagnostics.HasWarnings && mapObjectsCreatedSoFar.Count == 0 && typeMap.Count == 0;
        internal InjectionState Clone()
        {
            return new InjectionState(
              new DiagnosticBuilder(this.Diagnostics).Diagnostics
              , new WouldBeImmutableDictionary<(Type beanType, string beanName), Type>(this.typeMap)
              , new Dictionary<(Type, string), object>(mapObjectsCreatedSoFar)
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
        internal IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> TypeMap => typeMap;
        internal IDictionary<(Type, string), object> MapObjectsCreatedSoFar => mapObjectsCreatedSoFar;
        /// <summary>
        /// shortcut to diagnostics.AllToString()
        /// <see cref="PureDI.Diagnostics.AllToString"/>
        /// </summary>
        /// <returns></returns>
        public string AllDiagnosticsToString() => diagnostics.AllToString();
        /// <summary>
        /// shortcut to diagnostics.ToString()
        /// <see cref="PureDI.Diagnostics.ToString"/>
        /// </summary>
        /// <returns></returns>
        public string WarningsToString() => diagnostics.ToString();
        /// <summary>
        /// convenient starting point and useful for library user edge
        /// case where they want to call the container multiple times
        /// with no history.
        /// </summary>
        public static InjectionState Empty
          => new InjectionState(
                    new DiagnosticBuilder().Diagnostics
                    ,new WouldBeImmutableDictionary<(Type beanType, string beanName), Type>()
                    ,new Dictionary<(Type, string), object>()
                    );
    }
}