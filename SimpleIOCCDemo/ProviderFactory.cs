using com.TheDisappointedProgrammer.IOCC;

namespace SimpleIOCCDemo
{
	[Bean]
	public class ProviderFactory : IFactory
	{
		[BeanReference] private SimpleIOCContainer iocContainer = null;
		public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
		{
			if (System.Environment.GetCommandLineArgs().Length > 1)
			{
				return iocContainer.CreateAndInjectDependencies<FileListProvider>(injectionState);
			}
			else
			{
				return iocContainer.CreateAndInjectDependencies<UsageListProvider>(injectionState, rootBeanName: "usage");
			}
			
		}
	}
}
