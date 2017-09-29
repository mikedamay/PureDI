using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
	[Bean]
	public class ProviderFactory : IFactory
	{
		[BeanReference] private SimpleIOCContainer iocContainer = null;
		public object Execute(BeanFactoryArgs args)
		{
			if (System.Environment.GetCommandLineArgs().Length > 1)
			{
				return iocContainer.CreateAndInjectDependenciesSimple<FileListProvider>();
			}
			else
			{
				return iocContainer.CreateAndInjectDependenciesSimple<UsageListProvider>(beanName: "usage");
			}
			
		}
	}
}
