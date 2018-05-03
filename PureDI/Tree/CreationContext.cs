using System;
using System.Collections.Generic;

namespace PureDI.Tree
{
        internal class CreationContext
        {
            public CycleGuard CycleGuard { get; }
            public ISet<Type> BeansWithDeferredAssignments { get; }

            public CreationContext(CycleGuard cycleGuard, ISet<Type> beansWithDeferredAssignments)
            {
                CycleGuard = cycleGuard;
                BeansWithDeferredAssignments = beansWithDeferredAssignments;
            }
        }
    }
