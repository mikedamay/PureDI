using PureDI;

namespace SimpleIOCCDemo
{
	[Bean]
	public class ProviderFactory : IFactory
	{
		[BeanReference] private PDependencyInjector iocContainer = null;
		public (object bean, InjectionState injectionState) Execute(InjectionState injectionState, BeanFactoryArgs args)
		{
			if (System.Environment.GetCommandLineArgs().Length > 1)
			{
				return iocContainer.CreateAndInjectDependencies<FileListProvider>(injectionState);
			}
			else
			{
				return iocContainer.CreateAndInjectDependencies<UsageListProvider>(injectionState, rootBeanSpec: new RootBeanSpec(rootBeanName: "usage"));
			}
			
		}
	}
}
