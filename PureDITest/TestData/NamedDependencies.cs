using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PureDI;
using PureDI.Attributes;

// this module is not compiled as part of the project.  It is embedded as a resource.
namespace IOCCTest.TestData
{
    [Bean(Name="dep-name-abc")]
    public class NamedDependencies
    {
    }
    [Bean(Name="dep-name-xyz")]
    public class NamedDependencies1 : ISecond
    {
        
    }
    [Bean(Name="dep-name-def")]
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
