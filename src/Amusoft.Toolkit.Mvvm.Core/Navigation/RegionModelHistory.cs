// This file is licensed to you under the MIT license.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class RegionModelHistory : IRegionModelHistory
{
	private readonly ILogger<RegionModelHistory> _logger;
	private readonly IRegionModelStore _regionModelStore;

	public RegionModelHistory(ILogger<RegionModelHistory> logger, IRegionModelStore regionModelStore)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_regionModelStore = regionModelStore ?? throw new ArgumentNullException(nameof(regionModelStore));
	}

	public object? GetLastRegionModel(string regionName)
	{
		return _regionModelStore.GetLastModel(regionName);
	}

	public void PushEntry(string regionName, object model)
	{
		_logger.LogDebug("Pushing model of type {Type} to region {Name} is being cleared.", model.GetType().Name, regionName);
		_regionModelStore.PushModel(regionName, model);
	}

	public void Clear(string regionName)
	{
		_logger.LogDebug("The history for region {Name} is being cleared.", regionName);
		_regionModelStore.Clear(regionName);
	}

	public bool HasHistory(string regionName)
	{
		var hasEntries = _regionModelStore.HasEntries(regionName);
		_logger.LogDebug("Querying region {Name} if it has entries - {Result}", regionName, hasEntries ? "Yes" : "No");
		return hasEntries;
	}
}