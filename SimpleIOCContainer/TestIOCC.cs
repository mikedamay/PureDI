using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.TheDisappointedProgrammer.IOCC
{
    public class TestIOCC
    {
        [IOCCInjectedDependency] public ChildOne childOne;
    }

    public class ChildOne
    {
        
    }
}
