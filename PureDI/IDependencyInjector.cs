using System;
using System.Reflection;
using PureDI.Attributes;

namespace PureDI
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDependencyInjector
    {
        /// <summary>
        /// Creates an object of TRootType and then recursively creates and hooks up its dependencies
        /// </summary>
        /// <typeparam name="TRootType">Typically, the root node of a tree of objects </typeparam>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take the previous saved instance 
        ///     of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///     Pass null if no additional assemblies are required. 
        ///     The assembly in which the call to this method is made is included by default
        ///     irrespective of the argument passed here</param>
        /// <param name="rootBeanSpec">optional arguments which help identify the class of the object to be instantiated
        ///     at the root of the object graph</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        (TRootType rootBean, InjectionState injectionState)
            CreateAndInjectDependencies<TRootType>(InjectionState injectionState = null
                , Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null
            );

        /// <summary>
        /// Causes classes to be instantiated and injected, starting with the rootType.
        /// </summary>
        /// <param name="rootType">Typically, the root node of a tree of objects to be created by this call </param>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///     Pass null if no additional assemblies are required. 
        ///     The assembly in which the call to this method is made is included by default
        ///     irrespective of the argument passed here</param>
        /// <param name="rootBeanSpec">optional arguments which help identify the class of the object to be instantiated
        ///     at the root of the object graph</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        (object rootBean, InjectionState injectionState)
            CreateAndInjectDependencies(Type rootType, InjectionState injectionState = null
                , Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null
            );

        /// <summary>
        /// Causes classes to be instantiated and injected, starting with the rootType.
        /// </summary>
        /// <param name="rootTypeName">Typically, the root node of a tree of objects to be created by this call </param>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///     Pass null if no additional assemblies are required. 
        ///     The assembly in which the call to this method is made is included by default
        ///     irrespective of the argument passed here</param>
        /// <param name="rootBeanSpec">optional arguments which help identify the class of the object to be instantiated
        ///     at the root of the object graph</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        (object rootBean, InjectionState injectionState) CreateAndInjectDependencies(string rootTypeName
            , InjectionState injectionState = null, Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null
        );

        /// <summary>
        /// This version of the injection method allows the library user to instantiate an object
        /// using "new" or by whatever other means and have it injected into the object tree.
        /// It may be appropriate to make one or more calls to create objects that will be needed in the tree
        /// before using a different call to create the full tree.
        /// Currently this method will attempt to recursively inject dependencies for rootObject and
        /// this may not be the desired behaviour.  This is a shortcoming and will be addressed
        /// in a future version.
        /// The rules for handling CreateAndInjectDependencies for an instantiated object are a little complicated:
        /// a) Objects created or injected using any overload of CreateAndInjectDependencies should not be
        ///    passed as a root object to this overload.  No exception is thrown but warnings that
        ///    members have already been assigned may be added to the diagnostic results.
        /// b) If a singleton object is passed to this overload for a class that has already been instantiated
        ///    by the injector then an exception is thrown.
        /// c) It is pointless (but not penalised) to pass a prototype object with deferred injections.
        /// d) If the RootObject is passed as a singleton but its class is not annotated as a bean then
        ///    a warning will be recorded but it will be instantiated.
        /// e) Bean name and constructor name on prootype root objects are ignored as they will never be used.
        /// </summary>
        /// <param name="rootObject">some instantiated object which the library user needs
        ///     to attach to the object tree</param>
        /// <param name="injectionState">This is null the first time the method is called.
        ///     Subsequent calls will typically take some saved instance of injection state.</param>
        /// <param name="assemblies">an array of assemblies where beans to be injected will be found.
        ///     Pass null if no additional assemblies are required. 
        ///     The assembly in which the call to this method is made is included by default
        ///     irrespective of the argument passed here</param>
        /// <param name="rootBeanSpec"></param>
        /// <param name="deferDepedencyInjection">when set to true no attempt will be made to
        ///   create dependencies and assign them to rootObject's members.  The possible use case
        ///   is one in which there are cyclical dependencies which cannot be accommodated by
        ///   more normal processing</param>
        /// <returns>an object of rootType for use by the program and an injection state object which can
        ///   be passed into subsequent calls to Create...Dependencies if there are other program entry points
        ///   which require additional objects to be created.</returns>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        (object rootBean, InjectionState injectionState)
            CreateAndInjectDependencies(object rootObject
                ,InjectionState injectionState = null, Assembly[] assemblies = null, RootBeanSpec rootBeanSpec = null
                ,bool deferDepedencyInjection = false);

        /// <summary>
        /// This is the PureDI's equivalent of 'new'.  It instantiates an object which is returned.
        /// If the object has a bean defined for it then any active profile will be respected.
        /// If the object has no bean defined for it then this method will simply create the object usiong
        /// a no-arg constructor.
        /// In order for this object to participate in the injection mechanism a subsequent call
        /// to CreateAndInjectDependencies must be made using the overload that takes a root object rather
        /// than a type.
        /// </summary>
        /// <typeparam name="TBean">The type of the bean to be created.  It must have a no-arg constructor</typeparam>
        /// <returns>an object with no dependencies injected</returns>
        (TBean, InjectionState) CreateBean<TBean>(InjectionState injectionState = null) where TBean : class;

    }
}