using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData
{
    [IOCCBean(Name="dep-name-abc")]
    public class NamedDependencies
    {
    }
    [IOCCBean(Name="dep-name-xyz")]
    public class NamedDependencies1 : ISecond
    {
        
    }
    [IOCCBean(Name="dep-name-def")]
    public class NamedDependencies2 : INamedDependencies, ISecond
    {
    }

    public interface INamedDependencies
    {
    }

    public interface ISecond
    {
        
    }
}
