// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;
using Amusoft.Toolkit.Mvvm.IntegrationTests.Extensions;

using Microsoft.Extensions.DependencyInjection;

using Shouldly;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests;

public class ServiceCollectionTests : IntegrationTestBase
{
	[Fact]
	public void TestConfiguredCall()
	{
		var serviceCollection = new ServiceCollection();
		serviceCollection.AddIntegrationTestDefaults(d => {});
		var sp = serviceCollection.BuildServiceProvider();
		var ns = sp.GetRequiredService<INavigationService>();
		ns.ShouldNotBeNull();
	}
}