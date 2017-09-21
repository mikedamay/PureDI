using System;
using com.TheDisappointedProgrammer.IOCC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

public class Profiles
{
    public static void Main()
    {
        SimpleIOCContainer sic = new SimpleIOCContainer();
        MyService ms = sic.CreateAndInjectDependencies<MyService>();
        Console.WriteLine(ms.DoStuff());   // prints "doing the real thing"
    }
}
[Bean]
public class MyService
{
    [BeanReference] private IMyRepository myRepository;
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
            , new SimpleIOCContainer(Profiles: new[] { "test" }).CreateAndInjectDependencies<MyService>().DoStuff()
        );
}

namespace ProfilesRunner{ using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass] public class MainRunner{
[TestMethod] public void RunMain() => Profiles.Main();}}
