using PureDI;
using PureDI.Attributes;

namespace PureDITest.TestData
{
    public class NonBeanDerivedFromBean
    {
        [Bean]
        public class BeanClass
        {
            
        }

        public class NonBeanClass : BeanClass
        {
            
        }
    }
}