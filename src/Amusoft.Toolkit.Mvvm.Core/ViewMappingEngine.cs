using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.Logging;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class ViewMappingEngine : IViewMappingEngine
{
	private readonly ILogger<ViewMappingEngine> _logger;
	private readonly ICompositeMappingTypeSourceContainer _mappingTypeSource;
	private readonly IViewModelToViewMapper[] _mappers;

	public ViewMappingEngine(ILogger<ViewMappingEngine> logger, IEnumerable<IViewModelToViewMapper> mappers, ICompositeMappingTypeSourceContainer mappingTypeSource)
	{
		if(mappers == null)
			throw new ArgumentNullException(nameof(mappers));
		
		_mappers = mappers.ToArray();
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_mappingTypeSource = mappingTypeSource ?? throw new ArgumentNullException(nameof(mappingTypeSource));
	}

	public HashSet<(Type view, Type viewModel)> GetMappings()
	{
		var results = new HashSet<(Type view, Type viewModel)>();
		foreach (var mappingTypeSource in _mappingTypeSource.GetSources())
		{
			foreach (var pattern in _mappers)
			{
				_logger.LogDebug("Collecting patterns through {Name}.", pattern.GetType().Name);
				var patternResult = pattern.GetResult(mappingTypeSource);
				foreach (var patternResults in patternResult.Matches)
				{
					if (results.Add((patternResults.viewType, patternResults.viewModelType)))
					{
						_logger.LogTrace("Mapping {ViewModel} to {View}",
							patternResults.viewModelType.FullName,
							patternResults.viewType.FullName
						);
					}
					else
					{
						_logger.LogWarning("Skipped adding mapping for {ViewModel} to {View} because that mapping already exists",
							patternResults.viewModelType.FullName,
							patternResults.viewType.FullName
						);
					}
				}

				foreach (var type in patternResult.MismatchedViews)
				{
					_logger.LogWarning("Found no match for view type {Name}", type.Name);
				}

				foreach (var type in patternResult.MismatchedViewModels)
				{
					_logger.LogWarning("Found no match for viewModel type {Name}", type.Name);
				}
			}
		}

		return results;
	}
}