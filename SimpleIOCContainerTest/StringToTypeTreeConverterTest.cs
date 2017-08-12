using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class StringToTypeTreeConverterTest
    {
        [TestMethod]
        public void ShouldConvertNonGenericType()
        {
            var sttc = new StringToTypeTreeConverter();
            com.TheDisappointedProgrammer.IOCC.TypeTree tt = sttc.Convert("MyClass");
            Assert.AreEqual("MyClass", tt.TypeFullName);
        }

        [TestMethod]
        public void ShouldConvertSimpleGenericType()
        {
            var sttc = new StringToTypeTreeConverter();
            com.TheDisappointedProgrammer.IOCC.TypeTree tt = sttc.Convert("MyClass<System.Int32>");
            Assert.AreEqual("MyClass", tt.TypeFullName);
            Assert.AreEqual(1, tt?.GenericArguments.Count);
            Assert.AreEqual("System.Int32", tt?.GenericArguments[0]);

        }
        [TestMethod]
        public void ShouldConvertGenericWithNultipleArguments()
        {
            var sttc = new StringToTypeTreeConverter();
            com.TheDisappointedProgrammer.IOCC.TypeTree tt = sttc.Convert("MyClass<System.Int32,string>");
            Assert.AreEqual("MyClass", tt.TypeFullName);
            Assert.AreEqual(2, tt?.GenericArguments.Count);
            Assert.AreEqual("System.Int32", tt?.GenericArguments[0]);
            Assert.AreEqual("string", tt?.GenericArguments[1]);

        }
    }
}