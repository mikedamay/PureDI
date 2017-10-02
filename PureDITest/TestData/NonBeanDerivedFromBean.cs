using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCContainerTest.TestData
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