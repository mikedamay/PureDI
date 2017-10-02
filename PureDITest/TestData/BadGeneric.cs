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

    [Bean]
    public class GenericDerived : GenericBase<int>
    {

    }


    [Bean]
    class Gen<T>
    {

    }

    [Bean]
    class GenUser : Gen<T>
    {

    }
    [Bean]
    class T
    {

    }

}
