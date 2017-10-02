using System.Dynamic;
using IOCCTest.TestCode;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.ConstructorTestData
{
    [Bean]
    public class MultipleConstructorsComplex : IResultGetter
    {
        [BeanReference] private PDependencyInjector container = null;
        [BeanReference(ConstructorName = "first")] private Constructed first = null;
        [BeanReference(ConstructorName = "second")] private Constructed second = null;
        [BeanReference(ConstructorName = "first")] private Constructed third = null;
        public object GetResults()
        {
            return null;
                // the ssembly loader failed when when we
                // assigned values to the fields of an ExpandoObject
                // in the same way as we do in a hundred places.
                // It complained about a missing something to do with Compiler.ArgumentInfo
                // This was after messing with experimental .NET versions
                // but no other tests seem affected.

        }

        private void DoStuff()
        {
            fun(container, first, second,third);
        }
        private void fun(PDependencyInjector pdi, Constructed ca, Constructed cb, Constructed cc)
        {
            
        }
    }
    [Bean]
    internal class Constructed
    {
        public FirstParam FirstParam { get; set; }
        public SecondParam SecondParam { get; set; }
        [Constructor(Name = "first")]
        public Constructed([BeanReference] FirstParam firstParam
        )
        {
            this.FirstParam = firstParam;
        }
        [Constructor(Name = "second")]
        public Constructed([BeanReference] SecondParam secondParam
        )
        {
            this.SecondParam = secondParam;
        }


    }
    [Bean]
    internal class FirstParam
    {
    }
    [Bean]
    internal class SecondParam
    {
    }
}