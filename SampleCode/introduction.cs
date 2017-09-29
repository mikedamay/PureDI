using com.TheDisappointedProgrammer.IOCC;
[Bean]
public class Program
{
    [BeanReference] private Logger logger = null;
    public static void Main()
    {
        SimpleIOCContainer sic = new SimpleIOCContainer();
        Program prog = sic.CreateAndInjectDependencies<Program>().rootObject;
        prog.SaySomething();
    }
    private void SaySomething()
    {
        logger.Log("Hello Simple");
    }
}
[Bean]
public class Logger
{
    public void Log(string message) => System.Console.WriteLine(message);
}
namespace IntroductionRunner{ using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass] public class MainRunner{
[TestMethod] public void RunMain() => Profiles.Main();}}
