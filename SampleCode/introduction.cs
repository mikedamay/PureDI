using PureDI;
using PureDI.Attributes;

[Bean]
public class Program
{
    [BeanReference] private Logger logger = null;
    public static void Main()
    {
        PDependencyInjector pdi = new PDependencyInjector();
        Program prog = pdi.CreateAndInjectDependencies<Program>()
          .rootBean;
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
