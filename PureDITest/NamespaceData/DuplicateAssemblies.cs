
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.DuplicateAssemblies
{
    [Bean]
    public class DuplicateAssemblies : IResultGetter
    {
        public object GetResults()
        {

            return null;
        }
    }
}