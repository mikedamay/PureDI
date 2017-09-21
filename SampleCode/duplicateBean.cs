using com.TheDisappointedProgrammer.IOCC;

[Bean]
class MyClass
{
    [BeanReference(Name = "Impl1")] private IMyInterface myInterface = null;
    public void UseReference()
    {
        myInterface.DoStuff();
    }
    public static void Main()
    {
        var sic = new SimpleIOCContainer();
        MyClass myClass = sic.CreateAndInjectDependencies<MyClass>();
        myClass.UseReference();     // will display "One"
    }
}
public interface IMyInterface
{
    void DoStuff();
}
[Bean(Name = "Impl1")]
public class OneImplementation : IMyInterface
{
    public void DoStuff()
    {
        System.Console.WriteLine("One");
    }
}
[Bean(Name = "Impl2")]
public class TwoImplementation : IMyInterface
{
    public void DoStuff()
    {
        System.Console.WriteLine("Two");
    }
}

namespace DuplicateBeanRunner{ using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass] public class MainRunner{
[TestMethod] public void RunMain() => Profiles.Main();}}
