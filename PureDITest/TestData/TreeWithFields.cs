﻿using PureDI;
using PureDI.Attributes;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData
{
    public class TreeWithFields
    {
        [BeanReference]
        public ChildOne childOne;
    }
    [Bean]
    public class ChildOne
    {
        
    }
}