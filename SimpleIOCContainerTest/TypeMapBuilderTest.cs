using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest
{
    [TestClass]
    public class TypeMapBuilderTest
    {
        [TestMethod]
        public void ShouldCrewateTypeMapFromThisAssembly()
        {
            var tmb = new TypeMapBuilder();
            var map = tmb.BuildTypeMapFromAssemblies(new List<Assembly>() {typeof(TreeWithFields).Assembly});
            Assert.AreEqual(1, map.Keys.Count);
        }
    }
}
