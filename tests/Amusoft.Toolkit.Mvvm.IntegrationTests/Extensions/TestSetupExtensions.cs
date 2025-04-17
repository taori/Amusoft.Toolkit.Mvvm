// This file is licensed to you under the MIT license.

using Amusoft.Toolkit.Mvvm.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.Mvvm.IntegrationTests.Extensions;

public static class TestSetupExtensions
{
	public static void AddIntegrationTestDefaults(this IServiceCollection serviceCollection)
	{
		serviceCollection.AddLogging(d => d.AddDebug());
		serviceCollection.AddMvvmCoreServices();
	}
}