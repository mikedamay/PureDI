using System;
using PureDI;
using PureDI.Attributes;

namespace IOCCTest.DifficultTypeTestData
{
    internal class SomeAttribute : Attribute
    {
        
    }
    public class ConstructorsWithMultipleAttributes
    {
        [Some]
        [Constructor]
        public ConstructorsWithMultipleAttributes()
        {
            
        }
        [Some]
        [Constructor]
        public ConstructorsWithMultipleAttributes(int ii)
        {
            
        }

        public ConstructorsWithMultipleAttributes(string abc)
        {
            
        }
    }
}