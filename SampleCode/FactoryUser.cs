﻿using System;
using PureDI;

[Bean]
public class FactoryUser
{
    [BeanReference(Factory = typeof(EnvironmentVariableFactory))]
      private IRepository2 repo = null;
    public static void Main()
    {
        var factoryUser = new PDependencyInjector().CreateAndInjectDependencies<FactoryUser>().rootBean;
        Console.WriteLine(factoryUser.repo.GetData());
                // will print null unless you happen to have an environment
                // variable called CONNECTION_STRING
    }
}
[Bean]
public class EnvironmentVariableFactory : IFactory
{
    public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
    {
        return (new Repository(Environment.GetEnvironmentVariable("CONNECTION_STRING")), injectionState);
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


namespace FactoryRunner
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass]
    public class MainRunner
    {
        [TestMethod] public void RunMain() => FactoryUser.Main();
    }
}
