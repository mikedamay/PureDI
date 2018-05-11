using System;
using System.Collections.Generic;

namespace PureDI.Public
{
        /// <summary>
        /// An object of this class is passed into the bean factory Execute method
        /// and should be passed ot any call to PDependencyInjector.CreateAndInject...
        /// It cannot be used in any other way
        /// </summary>
        public class CreationContext
        {
            internal CycleGuard CycleGuard { get; }
            internal ISet<Type> BeansWithDeferredAssignments { get; }

            internal CreationContext(CycleGuard cycleGuard, ISet<Type> beansWithDeferredAssignments)
            {
                CycleGuard = cycleGuard;
                BeansWithDeferredAssignments = beansWithDeferredAssignments;
            }
        }
    }
