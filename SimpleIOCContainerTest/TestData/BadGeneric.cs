using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    public class GenericBase<T>
    {

    }

    [IOCCDependency]
    public class GenericDerived : GenericBase<int>
    {

    }


    [IOCCDependency]
    class Gen<T>
    {

    }

    [IOCCDependency]
    class GenUser : Gen<T>
    {

    }
    [IOCCDependency]
    class T
    {

    }

}
