﻿using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.DerivedAttributeTestData
{
    public class DerivedBeanAttribute : BeanBaseAttribute
    {
        
    }
    [DerivedBean]
    public class Bean : IResultGetter
    {
        [BeanReference] private Child child;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Child = child;
            return eo;
        }
    }

    [DerivedBean]
    public class Child
    {
        
    }
}