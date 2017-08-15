﻿using System;
using System.Dynamic;
using com.TheDisappointedProgrammer.IOCC;
using IOCCTest.TestCode;

namespace IOCCTest.FactoryTestData
{
    [IOCCBean]
    public class GenericFactory : IOCCFactory
    {
        public object Execute(BeanFactoryArgs args)
        {
            return new MyGeneric<int>();
        }
    }
    [IOCCBean]
    public class Generic : IResultGetter
    {
        [IOCCBeanReference(Factory=typeof(GenericFactory))]
        private MyGeneric<int> myGeneric;

        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.MyGeneric = myGeneric;
            return eo;
        }
    }
    internal class MyGeneric<T>
    {
    }
}