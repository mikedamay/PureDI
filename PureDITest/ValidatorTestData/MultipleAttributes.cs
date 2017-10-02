using System;
using PureDI;

namespace IOCCTest.DifficultTypeTestData
{
    public class SomeAttribute : Attribute {
    
    }

    public class MultipleAttributes
    {
        [Some] [BeanReference] private int someINt;
    }
}