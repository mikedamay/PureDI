using com.TheDisappointedProgrammer.IOCC;

namespace IOCCTest.TestData
{
    [IOCCDependency(OS = IOCC.OS.Linux, Profile = "someProfile")]
    public class CheckProfileAndOs : Interface1
    {

    }
    [IOCCDependency(Name = "mike", OS = IOCC.OS.Linux, Profile = "someProfile")]
    public class CheckProfileAndOs2 : Interface1
    {

    }
    [IOCCDependency(Name = "mike", OS = IOCC.OS.Windows, Profile = "someOtherProfile")]
    public class CheckProfileAndOs3 : Interface1
    {

    }
    [IOCCDependency(OS = IOCC.OS.Windows, Profile = "someProfile")]
    public class CheckProfileAndOs4 : Interface1
    {

    }
    [IOCCDependency(Name = "mike", OS = IOCC.OS.Windows, Profile = "someProfile")]
    public class CheckProfileAndOs5 : Interface1
    {

    }

    interface Interface1
    {
        
    }
}