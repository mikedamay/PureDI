using System;
using System.Collections.Generic;

namespace com.TheDisappointedProgrammer.IOCC.Tree
{
        internal class CreationContext
        {
            public IDictionary<Type, object> MapObjectsCreatedSoFar { get; }
            public CycleGuard CycleGuard { get; }
            public ISet<Type> CyclicalDependencies { get; }

            public CreationContext(
                IDictionary<Type, object> mapObjectsCreatedSoFar
                , CycleGuard cycleGuard
                , ISet<Type>cyclicalDependencies
            )
            {
                MapObjectsCreatedSoFar = mapObjectsCreatedSoFar;
                CycleGuard = cycleGuard;
                CyclicalDependencies = cyclicalDependencies;
            }
        }
    }
