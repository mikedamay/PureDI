using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.FactoryTestData
{
    public class FactoryWithoutFactoryAttribute {
    
    }
    [IOCCBean]
    public class MissingFactory
    {
        [IOCCBeanReference(Factory 
          = typeof(FactoryWithoutFactoryAttribute))] private int Abc;
    }
}