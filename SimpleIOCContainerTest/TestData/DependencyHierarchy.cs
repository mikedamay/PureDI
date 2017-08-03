using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using com.TheDisappointedProgrammer.IOCC;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData.DependencyHierarchy
{
    [IOCCDependency]
    public class ForstGemClassWithManyAncestors8 : SecondGenClass2 , ISecondGenWithAncestors2, ISecondGen3
    {
    }
    public class SecondGenClass2 : IThirdGen
    {
    }

    public interface ISecondGenWithAncestors2 : IThirdGenA
    {
    }

    public interface ISecondGen3 : IThirdGenB, IThirdGenC
    {
        
    }

    public interface IThirdGen
    {
        
    }
    public interface IThirdGenA
    {
        
    }
    public interface IThirdGenB
    {

    }
    public interface IThirdGenC
    {

    }

}
