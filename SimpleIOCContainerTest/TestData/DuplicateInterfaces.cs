using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData.DuplicateInterfaces
{
    [IOCCBean]
    class DuplicateInterfaces3 : FirstGen2, SecondGen1
    {
    }

    internal interface SecondGen1
    {
    }

    internal interface FirstGen2 : SecondGen1
    {
    }
}
