#region main
using PureDI;
using PureDI.Attributes;

[Bean]
public class BeanNamesDemo
{
    [BeanReference] private IRepository mainDB = null;
    [BeanReference(Name = "mongo")] private IRepository mongoDB = null;

    public static void Main()
    {
        var bean = new DependencyInjector()
          .CreateAndInjectDependencies<BeanNamesDemo>()
          .rootBean;
        bean.ListDatabases();
    }
    private void ListDatabases()
    {
        System.Console.WriteLine($"our main database is {mainDB.Id}");
        // this will display "SqlServerDB"
        System.Console.WriteLine($"our document database is {mongoDB.Id}");
        // this will display "magnificent mongo""
    }
}

internal interface IRepository
{
    string Id { get; }
}
[Bean]
internal class SqlServerDB : IRepository
{
    public string Id => "SqlServerDB";
}

[Bean(Name = "Mongo")]
internal class MongoDB : IRepository
{
    public string Id => "magnificent mongo";
}
#endregion
namespace BeanNamesDemoRunner
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass] public class MainRunner    {
[TestMethod] public void RunMain() => BeanNamesDemo.Main();}}

