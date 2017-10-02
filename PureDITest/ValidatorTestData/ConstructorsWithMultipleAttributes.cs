using System;
using PureDI;

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