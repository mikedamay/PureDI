using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IOCCTest
{
    [TestClass]
    public class TypeMatcherTest
    {
        private class MyClass
        {
            
        }
        [TestMethod]
        public void shouldMatchSimpleType()
        {
            TypeMatcher tm = new TypeMatcher();
            bool result = tm.Match(typeof(MyClass), new TypeNameTree("IOCCTest.TypeMatcherTest+MyClass"));
            Assert.IsTrue(result);
        }

        private class MyGeneric<T>
        {
            
        }
        private class MyDerived : MyGeneric<int>
        {
            
        }

        [TestMethod]
        public void ShouldMatchSimpleGeneric()
        {
            TypeMatcher tm = new TypeMatcher();
            bool result = tm.Match(typeof(MyDerived).BaseType
              ,new TypeNameTree("IOCCTest.TypeMatcherTest+MyGeneric<System.Int32>"));
            Assert.IsTrue(result);
        }

        private class MyGeneric<A, B>
        {
            
        }

        private class MyDerived2 : MyGeneric<int, MyGeneric<int>>
        {
            
        }
        [TestMethod]
        public void ShouldMatchMoreComplexGeneric()
        {
            TypeMatcher tm = new TypeMatcher();
            bool result = tm.Match(typeof(MyDerived2).BaseType
                , new TypeNameTree("IOCCTest.TypeMatcherTest+MyGeneric<System.Int32,IOCCTest.TypeMatcherTest+MyGeneric<System.Int32>>"));
            Assert.IsTrue(result);
            bool result2 = tm.Match(typeof(MyDerived2).BaseType
                , new TypeNameTree("IOCCTest.TypeMatcherTest+MyGeneric<IOCCTest.TypeMatcherTest+MyGeneric<System.Int32>,System.Int32>"));
            Assert.IsTrue(result2);
        }
        [TestMethod]
        public void ShouldRejectNonMatchingTypes()
        {
            TypeMatcher tm = new TypeMatcher();
            bool result = tm.Match(typeof(MyDerived2).BaseType
                , new TypeNameTree("IOCCTest.TypeMatcherTest+MyGeneric<int,IOCCTest.TypeMatcherTest+MyGeneric<System.Int32>>"));
            Assert.IsFalse(result);
        }
         [TestMethod]
        public void ShouldRejectBadlyFormedTypeSpec()
        {
            TypeMatcher tm = new TypeMatcher();
            bool result = tm.Match(typeof(MyDerived2).BaseType
                , new TypeNameTree("<<>>,abc"));
            Assert.IsFalse(result);
        }
   }
}