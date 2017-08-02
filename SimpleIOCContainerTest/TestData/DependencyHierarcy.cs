using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData
{
    [IOCCDependency(Name="dep-name-abc")]
    public class HierarchyNamed
    {
    }
    [IOCCDependency]
    public class HierarchyUnnamed : IFirstGen
    {
        
    }
    [IOCCDependency(Name="dep-name-def")]
    public class NamedDependencies2 : INamedDependencies, ISecondGen
    {
    }

    public class FirsstGenClass 
    {
    }

    public interface INamedDependencies
    {
    }

    public interface ISecondGen
    {
        
    }

    public interface IFirstGen
    {
        
    }
}
