using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
	[IOCCBean]
	public class ProviderFactory : IOCCFactory
	{
		[IOCCBeanReference] private SimpleIOCContainer iocContainer;
		public object Execute(BeanFactoryArgs args)
		{
			if (System.Environment.GetCommandLineArgs().Length > 1)
			{
				return iocContainer.GetOrCreateObjectTree<FileListProvider>();
			}
			else
			{
				return iocContainer.GetOrCreateObjectTree<UsageListProvider>(beanName : "usage");
			}
			
		}
	}
}
