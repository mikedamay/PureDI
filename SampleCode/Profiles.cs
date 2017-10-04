using System;
using PureDI;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class Profiles
{
    public static void Main()
    {
        PDependencyInjector pdi = new PDependencyInjector();
        MyService ms = pdi.CreateAndInjectDependencies<MyService>()
          .rootBean;
        Console.WriteLine(ms.DoStuff());   // prints "doing the real thing"
    }
}
[Bean]
public class MyService
{
    [BeanReference] private IMyRepository myRepository = null;
    public string DoStuff()
    {
        return myRepository.DoSomething();
    }
}
public interface IMyRepository
{
    string DoSomething();
}
[Bean]
public class MyRepository : IMyRepository
{
    public string DoSomething()
    {
        return "doing the real thing";
    }
}
[Bean(Profile = "test")]
public class MyTestRepository : IMyRepository
{
    public string DoSomething()
    {
        return "this is just a test";
    }
}
[TestClass]
public class SomeTest
{
    [TestMethod]
    public void ShouldPrintThisIsJustATest() =>
        Assert.AreEqual("this is just a test"
            , new PDependencyInjector(
              Profiles: new[] { "test" }).CreateAndInjectDependencies
            <MyService>().rootBean.DoStuff()
        );
}

namespace ProfilesRunner{ using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass] public class MainRunner{
[TestMethod] public void RunMain() => Profiles.Main();}}
