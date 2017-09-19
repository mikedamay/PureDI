﻿using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.RootObject
{
    public class ConnectUp
    {
        [BeanReference] public ExistingChild connectedChild;
    }

    [Bean]
    public class ExistingRoot
    {
        [BeanReference(Scope=BeanScope.Singleton)] public ExistingChild existingChild;
    }
    [Bean]
    public class ExistingChild
    {
    }
}