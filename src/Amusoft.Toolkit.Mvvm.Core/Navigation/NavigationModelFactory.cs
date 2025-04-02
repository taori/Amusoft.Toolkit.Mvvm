using System;
using System.Collections.Generic;
using System.Linq;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class NavigationModelFactory : INavigationModelFactory
{
	private readonly IActivator _activator;
	private readonly IRestoreStrategyProvider[] _restoreStrategyProviders;

	public NavigationModelFactory(IEnumerable<IRestoreStrategyProvider> restoreStrategyProviders, IActivator activator)
	{
		if(restoreStrategyProviders == null)
			throw new ArgumentNullException(nameof(restoreStrategyProviders));
		
		_activator = activator ?? throw new ArgumentNullException(nameof(activator));
		_restoreStrategyProviders = restoreStrategyProviders.ToArray();
	}
	
	private IEnumerable<object> GetRestoreStrategies(Type modelType, IRestoreStrategyProvider provider)
	{
		var method = provider.GetType().GetMethod(nameof(IRestoreStrategyProvider.GetStrategies));
		var specificMethod = method!.MakeGenericMethod(modelType);
		var strategies = specificMethod.Invoke(provider, null);
		return strategies as IEnumerable<object> ?? [];
	}

	public NavigationModel Create(object model)
	{
		var restoreStrategies = _restoreStrategyProviders
			.SelectMany(provider => GetRestoreStrategies(model.GetType(), provider))
			.ToArray();
		
		var castedStrategies = Array.CreateInstance(typeof(IRestoreStrategy<>).MakeGenericType(model.GetType()), restoreStrategies.Length);
		Array.Copy(restoreStrategies, castedStrategies, restoreStrategies.Length);

		var modelType = typeof(NavigationModel<>).MakeGenericType(model.GetType());
		var instance = _activator.CreateInstance(modelType, [model, castedStrategies]);
		if (instance is not NavigationModel navigationModel)
			throw new MvvmCoreException($"IActivator should have created an instance of type {modelType.Name} but instead it is {instance.GetType().Name}");

		return navigationModel;
	}
}