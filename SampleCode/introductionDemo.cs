#region main
using PureDI;
using PureDI.Attributes;

[Bean]
public class IntroductionDemo
{
    [BeanReference] private Logger logger = null;
    public static void Main()
    {
        DependencyInjector pdi = new DependencyInjector();
        var prog = pdi.CreateAndInjectDependencies<IntroductionDemo>()
          .rootBean;
        prog.SaySomething();  // prints "Hello Simple"
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
#endregion

public class Program {public static void Main() {}}
namespace IntroductionDemoRunner{ using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass] public class MainRunner{
[TestMethod] public void RunMain() => IntroductionDemo.Main();}}
