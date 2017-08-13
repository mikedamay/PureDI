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
            Assert.AreEqual("MyClass", tt?.TypeFullName);
            Assert.AreEqual(1, tt?.GenericArguments.Count);
            Assert.AreEqual("System.Int32", tt?.GenericArguments[0]?.TypeFullName);

        }
        [TestMethod]
        public void ShouldConvertGenericWithNultipleArguments()
        {
            var sttc = new StringToTypeTreeConverter();
            com.TheDisappointedProgrammer.IOCC.TypeTree tt = sttc.Convert("MyClass<System.Int32,string>");
            Assert.AreEqual("MyClass", tt?.TypeFullName);
            Assert.AreEqual(2, tt?.GenericArguments.Count);
            Assert.AreEqual("System.Int32", tt?.GenericArguments[0]?.TypeFullName);
            Assert.AreEqual("string", tt?.GenericArguments[1]?.TypeFullName);

        }

        [TestMethod]
        public void SHouldConvertGenericWithMultipleLevels()
        {
            var sttc = new StringToTypeTreeConverter();
            com.TheDisappointedProgrammer.IOCC.TypeTree tt = sttc.Convert("MyClass<MyClass2<System.Int32>,MyClass2<string>>");
            Assert.AreEqual("MyClass", tt.TypeFullName);
            Assert.AreEqual(2, tt?.GenericArguments.Count);
            Assert.AreEqual("MyClass2", tt?.GenericArguments[0]?.TypeFullName);
            Assert.AreEqual("MyClass2", tt?.GenericArguments[1]?.TypeFullName);
            Assert.AreEqual("System.Int32", tt?.GenericArguments[0]?.GenericArguments[0]?.TypeFullName);
            Assert.AreEqual("string", tt?.GenericArguments[1]?.GenericArguments[0]?.TypeFullName);

        }
        [TestMethod]
        public void SHouldConvertGenericWithMultipleLevels2()
        {
            var sttc = new StringToTypeTreeConverter();
            com.TheDisappointedProgrammer.IOCC.TypeTree tt = sttc.Convert("MyClass<MyClass2<System.Int32>,MyClass2<MyClass<string,string>>>");
            Assert.AreEqual("MyClass", tt.TypeFullName);
            Assert.AreEqual(2, tt?.GenericArguments.Count);
            Assert.AreEqual("MyClass2", tt?.GenericArguments[0]?.TypeFullName);
            Assert.AreEqual("MyClass2", tt?.GenericArguments[1]?.TypeFullName);
            Assert.AreEqual("System.Int32", tt?.GenericArguments[0]?.GenericArguments[0]?.TypeFullName);
            Assert.AreEqual("MyClass", tt?.GenericArguments[1]?.GenericArguments[0]?.TypeFullName);
        }

        [TestMethod]
        public void ShouldBuildTreeForBadlyFormedTypeSpec()
        {
            var sttc = new StringToTypeTreeConverter();
            com.TheDisappointedProgrammer.IOCC.TypeTree tt;
            tt = sttc.Convert("abc<,<");
            Assert.IsNotNull(tt);
            tt = sttc.Convert("");
            Assert.IsNotNull(tt);
            tt = sttc.Convert(",,,");
            Assert.IsNotNull(tt);
            tt = sttc.Convert("<<<");
            Assert.IsNotNull(tt);
            tt = sttc.Convert(">>>");
            Assert.IsNotNull(tt);
            tt = sttc.Convert(">>>,abc,>");
            Assert.IsNotNull(tt);
            tt = sttc.Convert("<abc>");
            Assert.IsNotNull(tt);
            tt = sttc.Convert("abc,def");
            Assert.IsNotNull(tt);
        }
    }
}