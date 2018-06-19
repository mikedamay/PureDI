#region main
using System;
using System.Linq;
using PureDI;
using PureDI.Attributes;

public class InjectionStateDemo
{
    public static void Main()
    {
        InjectionState injectionState;
        PermanentService ps;
        var pdi = new DependencyInjector();
        (ps, injectionState) = pdi.CreateAndInjectDependencies<PermanentService>();
        InjectionState transitoryState = injectionState;
        for (int ii = 0; ii < 10; ii++)
        {
            BigData bigData;
            (bigData, transitoryState)
                = pdi.CreateAndInjectDependencies<BigData>(injectionState);
            Console.WriteLine(bigData.AddUp());     // 500_000_500_000
                                                    // 500_000_500_000
                                                    // 500_000_500_000
                                                    // 500_000_500_000
                                                    // 500_000_500_000
                                                    // 500_000_500_000
                                                    // 500_000_500_000
                                                    // 500_000_500_000
                                                    // 500_000_500_000
                                                    // 500_000_500_000
        }
    }
}

[Bean]
public class PermanentService
{
    public int GetData(int ii)
    {
        return ii + 1;
    }
}

[Bean]
public class BigData
{
    private int[] blob = new int[1_000_000];
    [BeanReference] private PermanentService permanent = null;
    private decimal agg;

    public decimal AddUp()
    {        
        for (int ii = 0; ii < blob.Length; ii++)
        {
            blob[ii] = permanent.GetData(ii);
        }
        blob.Sum(b => agg += b);
        return agg;
    }
}
#endregion
namespace InjectionStateDemoRunner
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    [TestClass]
    public class MainRunner
    {
        [TestMethod] public void RunMain() => InjectionStateDemo.Main();
    }
}
