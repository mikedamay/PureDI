using com.TheDisappointedProgrammer.IOCC;
using Microsoft.Office.Tools.Excel;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

[Bean]
public class DuplicateBean
{
    [BeanReference] private IRepository mainDB;
    [BeanReference(Name = "mongo")] private IRepository mongoDB;

    public static void Main()
    {
        var beans = new SimpleIOCContainer().CreateAndInjectDependencies<DuplicateBean>();
        beans.ListDatabases();
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
namespace BeanNamesRunner{using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass] public class MainRunner    {
[TestMethod] public void RunMain() => DuplicateBean.Main();}}

