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
    public class IgnoreReferencesAttributeTest
    {
        [TestMethod]
        public void ShouldCreateTreeForGenericDeclarations()
        {
            IDictionary<(string, string), string> mapExpected = new Dictionary<(string, string), string>()
            {
                {("IOCCTest.TestData.IgnoreHelper", ""), "IOCCTest.TestData.IgnoreHelper"}
            };
            TypeMapBuilderTest.CommonTypeMapTest("IOCCTest.TestData.IgnoreHelper.cs", mapExpected);

        }

    }
}