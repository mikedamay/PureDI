using System;
using BeanNamesRunner;
using com.TheDisappointedProgrammer.IOCC;

[Bean]
public class Constructor
{
    [BeanReference]
    private ISomeService someService = null;

    public static void Main()
    {
        var constructor = new PDependencyInjector()
          .CreateAndInjectDependencies<Constructor>().rootBean;
        constructor.someService.DoMeAFavour();   // writes "SomeData"
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

public interface ISomeRepository
{
    string GetSomeData();
    void Init(string connectionSTring);
}

namespace ConstructorRunner
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass]
    public class MainRunner
    {
        [TestMethod] public void RunMain() => Constructor.Main();
    }
}
