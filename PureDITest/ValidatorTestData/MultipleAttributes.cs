using System;
using PureDI;
using PureDI.Attributes;

namespace IOCCTest.DifficultTypeTestData
{
    public class SomeAttribute : Attribute {
    
    }

    public class MultipleAttributes
    {
        [Some] [BeanReference] private int someINt;
    }
}