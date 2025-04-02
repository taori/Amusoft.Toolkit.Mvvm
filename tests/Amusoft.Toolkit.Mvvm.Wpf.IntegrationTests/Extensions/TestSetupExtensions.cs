using Amusoft.Toolkit.Mvvm.Core;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.Mvvm.Wpf.IntegrationTests.Extensions;

public static class TestSetupExtensions
{
	public static void AddIntegrationTestDefaults(this IServiceCollection serviceCollection, Action<MvvmOptions>? setupAction = default)
	{
		serviceCollection.AddLogging(d => d.AddDebug());
		serviceCollection.AddMvvmWpf(setupAction);
	}
}