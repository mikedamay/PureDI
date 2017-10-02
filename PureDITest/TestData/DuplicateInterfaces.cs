using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PureDI;

namespace IOCCTest.TestData.DuplicateInterfaces
{
    [Bean]
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
