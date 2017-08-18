using System;
using System.Collections.Generic;

namespace com.TheDisappointedProgrammer.IOCC.Tree
{
        internal class CreationContext
        {
            public IDictionary<(Type constructableType, string constructorName)
              , object> MapObjectsCreatedSoFar { get; }
            public CycleGuard CycleGuard { get; }
            public ISet<Type> CyclicalDependencies { get; }

            public CreationContext(
                IDictionary<(Type, string), object> mapObjectsCreatedSoFar
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
