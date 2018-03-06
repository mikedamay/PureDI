using System.Dynamic;
using PureDI;
using PureDI.Attributes;
using IOCCTest.TestCode;

namespace IOCCTest.ProfileTestData
{
    [Bean]
    public class BestCandidate : IResultGetter
    {
        [BeanReference] private IBest best;
        public dynamic GetResults()
        {
            dynamic eo = new ExpandoObject();
            eo.Val = best.Val;
            return eo;
        }
    }

    public interface IBest
    {
        int Val { get; }
    }

    [Bean(Profile = "dobest")]
    public class BestCAndidateWithProfile : IBest
    {
        private int val = 42;
        public int Val => val;
    }

    [Bean]
    public class SecondBest : IBest
    {
        public int val = 24;
        public int Val => val;
    }
    [Bean(Profile="thirdbest")]
    public class ThirdBest : IBest
    {
        public int val = 33;
        public int Val => val;
    }

}