using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class RegionModelStore : IRegionModelStore
{
	private readonly INavigationModelFactory _navigationModelFactory;
	private readonly ConcurrentDictionary<string, Stack<NavigationModel>> _modelHistoryByRegionName = new();

	public RegionModelStore(INavigationModelFactory navigationModelFactory)
	{
		_navigationModelFactory = navigationModelFactory ?? throw new ArgumentNullException(nameof(navigationModelFactory));
	}

	public object? GetLastModel(string regionName)
	{
		if (!_modelHistoryByRegionName.TryGetValue(regionName, out var history))
			throw new MvvmCoreException($"Region \"{regionName}\" does not exist");
		if(!history.TryPop(out var navModel))
			throw new MvvmCoreException($"The history for \"{regionName}\" does not contain entries");

		return navModel.GetModel();
	}

	public void PushModel(string regionName, object model)
	{
		var history = _modelHistoryByRegionName.GetOrAdd(regionName, _ => new());
		var navModel = _navigationModelFactory.Create(model);
		history.Push(navModel);
	}

	public void Clear(string regionName)
	{
		if (!_modelHistoryByRegionName.TryGetValue(regionName, out var history))
		{
			throw new MvvmCoreException("The history for region {Name} is not registered and therefore cannot be cleared.");
		}
		
		history.Clear();
	}

	public bool HasEntries(string regionName)
	{
		return _modelHistoryByRegionName.TryGetValue(regionName, out var history) 
		       && history.Count > 0;
	}
}