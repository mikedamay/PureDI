using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
	[Bean]
	public class ProviderFactory : IFactory
	{
		[BeanReference] private SimpleIOCContainer iocContainer;
		public object Execute(BeanFactoryArgs args)
		{
			if (System.Environment.GetCommandLineArgs().Length > 1)
			{
				return iocContainer.CreateAndInjectDependencies<FileListProvider>();
			}
			else
			{
				return iocContainer.CreateAndInjectDependencies<UsageListProvider>(beanName : "usage");
			}
			
		}
	}
}
