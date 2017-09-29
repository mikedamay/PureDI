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
				return iocContainer.CreateAndInjectDependencies<FileListProvider>().rootObject;
			}
			else
			{
				return iocContainer.CreateAndInjectDependencies<UsageListProvider>(rootBeanName: "usage").rootObject;
			}
			
		}
	}
}
