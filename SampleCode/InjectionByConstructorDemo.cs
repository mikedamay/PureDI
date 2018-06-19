#region main
using System;
using PureDI;
using PureDI.Attributes;

[Bean]
public class InjectionByConstructorDemo
{
    [BeanReference]
    private ISomeService someService = null;

    public static void Main()
    {
        var constructorUser = new DependencyInjector()
          .CreateAndInjectDependencies<
          InjectionByConstructorDemo>().rootBean;
        constructorUser.someService.DoMeAFavour();   // writes "SomeData"
    }
}

public interface ISomeService
{
    void DoMeAFavour();
}

[Bean]
public class SomeService : ISomeService
{
    private ISomeRepository _repo;
    [Constructor]
    public SomeService([BeanReference] ISomeRepository repo)
    {
        repo.Init("myConnection");
        _repo = repo;
    }
    public void DoMeAFavour()
    {
        Console.WriteLine(_repo.GetSomeData()); 
    }

}

public interface ISomeRepository
{
    string GetSomeData();
    void Init(string connectionSTring);
}
[Bean]
public class SomeRepository : ISomeRepository
{
    private string _connectionString;

    public string GetSomeData()
    {
        return "someData";
    }

    public void Init(string initString)
    {
        _connectionString = initString;
    }
}
#endregion
namespace InjectionByConstructorDemoRunner
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass]
    public class MainRunner
    {
        [TestMethod] public void RunMain() => InjectionByConstructorDemo.Main();
    }
}
