using PureDI;
using PureDI.Attributes;

namespace SimpleIOCCDemo
{
	[Bean]
	public class ProviderFactory : IFactory
	{
		[BeanReference] private DependencyInjector iocContainer = null;
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
