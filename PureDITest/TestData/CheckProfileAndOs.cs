using PureDI;

namespace IOCCTest.TestData
{
    [Bean(OS = PDependencyInjector.OS.Linux, Profile = "someProfile")]
    public class CheckProfileAndOs
    {

    }
    [Bean( OS = PDependencyInjector.OS.Windows, Profile = "someProfile")]
    public class CheckProfileAndOs2
    {

    }
    [Bean( OS = PDependencyInjector.OS.Any, Profile = "someProfile")]
    public class CheckProfileAndOs3
    {

    }
    [Bean(OS = PDependencyInjector.OS.Linux, Profile = "someOtherProfile")]
    public class CheckProfileAndOs4
    {

    }
    [Bean(OS = PDependencyInjector.OS.Windows, Profile = "someOtherProfile")]
    public class CheckProfileAndOs5
    {

    }
    [Bean(OS = PDependencyInjector.OS.Any, Profile = "someOtherProfile")]
    public class CheckProfileAndOs6
    {

    }
    [Bean(OS = PDependencyInjector.OS.Linux, Profile = "")]
    public class CheckProfileAndOs7
    {

    }
    [Bean(OS = PDependencyInjector.OS.Windows, Profile = "")]
    public class CheckProfileAndOs8
    {

    }
    [Bean(OS = PDependencyInjector.OS.Any, Profile = "")]
    public class CheckProfileAndOs9
    {

    }

}