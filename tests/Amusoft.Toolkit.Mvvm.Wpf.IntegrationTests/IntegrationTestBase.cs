// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests.Extensions;

using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests;

public class IntegrationTestBase
{
	protected IServiceProvider GetServiceProvider(Action<MvvmOptions>? wpfConfiguration = default, Action<IServiceCollection>? serviceSetup = default)
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddIntegrationTestDefaults(wpfConfiguration);
		serviceSetup?.Invoke(serviceCollection);
		var serviceProvider = serviceCollection.BuildServiceProvider();
		return serviceProvider;
	}
}