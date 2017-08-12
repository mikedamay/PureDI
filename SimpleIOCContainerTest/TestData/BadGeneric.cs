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

    [IOCCBean]
    public class GenericDerived : GenericBase<int>
    {

    }


    [IOCCBean]
    class Gen<T>
    {

    }

    [IOCCBean]
    class GenUser : Gen<T>
    {

    }
    [IOCCBean]
    class T
    {

    }

}
