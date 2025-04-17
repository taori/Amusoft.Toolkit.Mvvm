// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.IntegrationTests.Extensions;

using Microsoft.Extensions.DependencyInjection;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests;

public class IntegrationTestBase
{
	protected INavigationService GetNavigationService(Action<ServiceCollection>? setup = default)
	{
		return ServiceProviderServiceExtensions.GetRequiredService<INavigationService>(GetServiceProvider(setup));
	}

	protected IServiceProvider GetServiceProvider(Action<ServiceCollection>? setup = default)
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddIntegrationTestDefaults();
		setup?.Invoke(serviceCollection);
		var serviceProvider = serviceCollection.BuildServiceProvider();
		return serviceProvider;
	}
}