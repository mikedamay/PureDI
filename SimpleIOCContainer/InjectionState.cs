using System;
using System.Collections.Generic;
using com.TheDisappointedProgrammer.IOCC.Common;

namespace com.TheDisappointedProgrammer.IOCC
{
    /// <summary>
    /// This class contains the history of dependencies injected in
    /// previous calls to <code>CreateAndInjectDependencies</code>.
    /// It is returned by <code>CreateAndInjectDependencies</code> and the expectation
    /// is that it will be passed to subsequent calls.  This pattern allows 
    /// multiple calls to share the same data whilst supporting thread safety and
    /// avoiding memory leaks where injections are transitory
    /// </summary>
    public class InjectionState
    {
        private readonly IOCCDiagnostics diagnostics;
        private readonly IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap;
        private readonly IDictionary<(Type, string), object> mapObjectsCreatedSoFar;
        internal InjectionState(IOCCDiagnostics diagnostics
          ,IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap
          ,IDictionary<(Type, string), object> mapObjectsCreatedSoFar)
        {
            this.diagnostics = diagnostics;
            this.mapObjectsCreatedSoFar = mapObjectsCreatedSoFar;
            this.typeMap = typeMap;
        }

        internal void Deconstructor(out IOCCDiagnostics diagnostics
            , out IWouldBeImmutableDictionary<(Type beanType, string beanName), Type> typeMap
            , IDictionary<(Type, string), object> mapObjectsCreatedSoFar)
        {
            diagnostics = this.diagnostics;
            mapObjectsCreatedSoFar = this.mapObjectsCreatedSoFar;
            typeMap = this.typeMap;

        }
        /// <summary>
        /// sophisticated library users may want to analyze diagnostics programmatically.
        /// Access to the diagnostics supports this endeavor.
        /// </summary>
        public IOCCDiagnostics Diagnostics => diagnostics;
        /// <summary>
        /// shortcut to diagnostics.AllToString()
        /// <see cref="IOCCDiagnostics.AllToString"/>
        /// </summary>
        /// <returns></returns>
        public string AllDiagnosticsToString() => diagnostics.AllToString();
        /// <summary>
        /// shortcut to diagnostics.ToString()
        /// <see cref="IOCCDiagnostics.ToString"/>
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