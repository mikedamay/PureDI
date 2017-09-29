using System;
using System.Collections.Generic;

namespace com.TheDisappointedProgrammer.IOCC.Tree
{
        internal class CreationContext
        {
            public CycleGuard CycleGuard { get; }
            public ISet<Type> CyclicalDependencies { get; }

            public CreationContext(CycleGuard cycleGuard, ISet<Type> cyclicalDependencies)
            {
                CycleGuard = cycleGuard;
                CyclicalDependencies = cyclicalDependencies;
            }
        }
    }
