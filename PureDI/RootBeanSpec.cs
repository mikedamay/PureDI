using System;
using PureDI.Attributes;
using PureDI.Common;

namespace PureDI
{
    /// <summary>
    /// An object of this type contains ancillary information to identify the "root" bean
    /// to be created.
    /// </summary>
    public class RootBeanSpec
    {
        /// <param name="rootBeanName">pass a bean name in the edge case when an interface
        /// or base class is passed as the root type but has multiple implementations</param>
        /// <param name="rootConstructorName">pass a constructor name in the edge case when 
        /// a class is being passed as the root type with multiple constructors</param>
        /// <param name="scope">See links below for an explanation of scope.  The scope passed in will apply to the 
        /// root bean only.  It has no effect on the rest of the tree.</param>
        /// <seealso cref="BeanReferenceAttribute">see BeanReference for an explanation of Scope</seealso>
        public RootBeanSpec(string rootBeanName = Constants.DefaultBeanName
            , string rootConstructorName = Constants.DefaultConstructorName, BeanScope scope = BeanScope.Singleton)
        {
            RootBeanName = rootBeanName == null ? throw new ArgumentNullException() : rootBeanName.ToLower();
            RootConstrutorName = rootConstructorName == null ? throw new ArgumentNullException() : rootConstructorName.ToLower();
            Scope = scope;
        }
        /// <summary>
        /// see constructor
        /// </summary>
        /// <param name="rootBeanName">see constructor</param>
        /// <param name="rootConstructorName">see constructor</param>
        /// <param name="scope">see constructor</param>
        public void Deconstruct(out string rootBeanName, out string rootConstructorName
            , out BeanScope scope)
        {
            rootBeanName = RootBeanName;
            rootConstructorName = RootConstrutorName;
            scope = Scope;
        }
        /// <summary>
        /// pass a bean name in the edge case when an interface
        /// or base class is passed as the root type but has multiple implementations
        /// </summary>
        public string RootBeanName { get; }
        /// <summary>
        /// pass a constructor name in the edge case when 
        /// a class is being passed as the root type with multiple constructors
        /// </summary>
        public string RootConstrutorName { get; }
        /// <summary>
        /// See links below for an explanation of scope.  The scope passed in will apply to the 
        /// root bean only.  It has no effect on the rest of the tree.
        /// </summary>
        public BeanScope Scope { get; }
    }
}