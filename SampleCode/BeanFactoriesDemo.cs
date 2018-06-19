#region main
using System;
using PureDI;
using PureDI.Attributes;

[Bean]
public class BeanFactoriesDemo
{
    [BeanReference(Factory = typeof(EnvironmentVariableFactory))]
      private IRepository2 repo = null;
    public static void Main()
    {
        var factoryUser = new DependencyInjector()
          .CreateAndInjectDependencies<BeanFactoriesDemo>().rootBean;
        Console.WriteLine(factoryUser.repo.GetData());
                // will print null unless you happen to have an environment
                // variable called CONNECTION_STRING
    }
}
[Bean]
public class EnvironmentVariableFactory : IFactory
{
    public (object bean, InjectionState injectionState) 
      Execute(InjectionState injectionState, BeanFactoryArgs args)
    {
        return (new Repository(Environment.GetEnvironmentVariable("CONNECTION_STRING"))
          , injectionState);
    }
}

public class Repository : IRepository2
{
    private readonly string _connectionString;
    public Repository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public string GetData()
    {
        return _connectionString;
    }
}

public interface IRepository2
{
    string GetData();
}
#endregion
namespace BeanFactoriesDemoRunner
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass]
    public class MainRunner
    {
        [TestMethod] public void RunMain() => BeanFactoriesDemo.Main();
    }
}
