using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using IOCCTest.TestCode;
using PureDI.Attributes;

namespace IOCCTest.GenericTestData
{
    [Bean]
    public class Generic : IResultGetter
    {
        [BeanReference]
        public IFoo<int> Fint;

        public dynamic GetResults()
        {
            dynamic o = new ExpandoObject();
            o.Value = Fint.Value;
            return o;
        }

    }

    public interface IFoo<T>
    {
        T Value { get; }
    }
    [Bean]
    public class Foo<T> : IFoo<T>
    {
        public T Value => default(T);
    }
}
