using System.Collections.Generic;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [IOCCIgnore]
    public interface MyIgnoredReference
    {
        
    }
    [TestClass]
    public class IgnoreAttributeTest
    {
        [TestMethod]
        public void souldIgnoreAncestorsMarkedAsIOCCIgnore()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.IgnoreHelper", ""), "IOCCTest.TestData.IgnoreHelper"}
            };
            TypeMapBuilderTest.CommonTypeMapTest($"{Utils.TestResourcePrefix}.TestData.IgnoreHelper.cs", mapExpected);

        }

    }
}