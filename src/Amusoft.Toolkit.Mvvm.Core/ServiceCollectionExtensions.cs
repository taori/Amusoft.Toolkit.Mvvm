using System;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Amusoft.Toolkit.Mvvm.Core;

internal static class ServiceCollectionExtensions
{
	internal static void AddMvvmCoreServices(this IServiceCollection services, Action<MvvmOptions>? setupAction = null)
	{
		if (setupAction is not null)
		{
			services.Configure(setupAction);
		}
		else
		{
			services.AddOptions<MvvmOptions>();
		}

		services.TryAddSingleton<ICompositeMappingTypeSourceContainer, CompositeMappingTypeSourceContainer>();
		services.TryAddSingleton<IViewModelToViewMapper, NamespaceConventionViewModelToViewMapper>();
		services.TryAddSingleton<IViewMappingEngine, ViewMappingEngine>();
		services.TryAddSingleton<INavigationService, NavigationService>();
		services.TryAddSingleton<IModelFactory, ModelFactory>();
		services.TryAddSingleton<IRestoreStrategyProvider, RestorePropertyRestoreStrategyProvider>();
		services.TryAddSingleton<IRegionRegister>(RegionRegister.Instance);
		services.TryAddSingleton<IRegionModelHistory, RegionModelHistory>();
		services.TryAddSingleton<IRegionModelStore, RegionModelStore>();
		services.TryAddSingleton<IActivator, Activator>();
		services.TryAddSingleton<INavigationModelFactory, NavigationModelFactory>();
	}
}