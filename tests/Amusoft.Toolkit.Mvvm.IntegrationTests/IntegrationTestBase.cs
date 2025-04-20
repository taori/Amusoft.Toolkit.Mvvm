// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.IntegrationTests.Extensions;

using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests;

public class IntegrationTestBase
{
	protected INavigationService GetNavigationService(Action<ServiceCollection>? setup = default, Action<MvvmOptions>? mvvmConfig = default)
	{
		return ServiceProviderServiceExtensions.GetRequiredService<INavigationService>(GetServiceProvider(setup, mvvmConfig));
	}

	protected IServiceProvider GetServiceProvider(Action<ServiceCollection>? setup = default, Action<MvvmOptions>? mvvmConfig = default)
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddIntegrationTestDefaults(mvvmConfig);
		setup?.Invoke(serviceCollection);
		var serviceProvider = serviceCollection.BuildServiceProvider();
		return serviceProvider;
	}
}