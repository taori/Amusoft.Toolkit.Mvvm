using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Amusoft.Toolkit.Mvvm.Core;

internal static class ServiceCollectionExtensions
{
	internal static void AddMvvmCoreServices(this IServiceCollection services)
	{
		services.TryAddSingleton<INavigationService, NavigationService>();
		services.TryAddSingleton<IRestoreStrategyProvider, RestorePropertyRestoreStrategyProvider>();
		services.TryAddSingleton<IMvvmMappingPattern, NamespaceConventionPattern>();
		services.TryAddSingleton<IDataTemplateSource, PatternMappingEngine>();
	}
}