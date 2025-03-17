using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class PatternMappingEngine : IDataTemplateSource
{
	private readonly IMvvmMappingPattern[] _patterns;
	private readonly ILogger<PatternMappingEngine> _logger;
	private readonly MvvmOptions _options;

	public PatternMappingEngine(IEnumerable<IMvvmMappingPattern> patterns, ILogger<PatternMappingEngine> logger, IOptions<MvvmOptions> options)
	{
		_patterns = patterns.ToArray();
		_logger = logger;
		_options = options.Value;
	}

	public HashSet<(Type view, Type viewModel)> GetMappings()
	{
		var results = new HashSet<(Type view, Type viewModel)>();
		foreach (var pattern in _patterns)
		{
			_logger.LogDebug("Collecting patterns through {Name}.", pattern.GetType().Name);
			var patternResult = pattern.GetResult(_options.MappingInput);
			foreach (var patternResults in patternResult.Matches)
			{
				if(!results.Add((patternResults.viewType, patternResults.viewModelType)))
					_logger.LogInformation("Skipped adding mapping for {View} to {ViewModel} because that mapping already exists", patternResults.viewType.FullName, patternResults.viewModelType.FullName);
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
		
		return results;
	}
}

/// <summary>
/// Provides an array of types that are mapped together as datatemplates
/// </summary>
internal interface IDataTemplateSource
{
	/// <summary>
	/// Gets an array for templates that have to be mapped
	/// </summary>
	/// <returns></returns>
	HashSet<(Type view, Type viewModel)> GetMappings();
}

/// <summary>
/// Inputs used for the mapping process
/// </summary>
public class MvvmMappingInput
{
	internal MvvmMappingInput()
	{
		ViewTypes = new (() => GetViewTypes().ToArray());
		ViewModelTypes = new (() => GetViewModelTypes().ToArray());
	}
	
	private readonly List<Assembly> _assemblies = [];
	private readonly List<Predicate<Type>> _viewFilters = [];
	private readonly List<Predicate<Type>> _viewModelFilters = [];
	internal Lazy<Type[]> ViewTypes;
	internal Lazy<Type[]> ViewModelTypes;

	/// <summary>
	/// Gets all view types
	/// </summary>
	/// <returns></returns>
	private IEnumerable<Type> GetViewTypes()
	{
		var types = _assemblies.SelectMany(a => a.GetTypes());
		foreach (var filter in _viewFilters)
		{
			types = types.Where(t => filter(t));
		}
		return types;
	}
	
	/// <summary>
	/// Gets all view types
	/// </summary>
	/// <returns></returns>
	private IEnumerable<Type> GetViewModelTypes()
	{
		var types = _assemblies.SelectMany(a => a.GetTypes());
		foreach (var filter in _viewModelFilters)
		{
			types = types.Where(t => filter(t));
		}
		return types;
	}

	/// <summary>
	/// Picks assemblies for the mapping process
	/// </summary>
	/// <param name="assembly"></param>
	/// <returns></returns>
	public MvvmMappingInput WithAssembly(Assembly assembly)
	{
		_assemblies.Add(assembly);
		return this;
	}

	/// <summary>
	/// Uses the given filter to narrow down view types
	/// </summary>
	/// <param name="filter"></param>
	/// <returns></returns>
	public MvvmMappingInput WithViewFilter(Predicate<Type> filter)
	{
		_viewFilters.Add(filter);
		return this;
	}

	/// <summary>
	/// Uses the given filter to narrow down view types
	/// </summary>
	/// <param name="filter"></param>
	/// <returns></returns>
	public MvvmMappingInput WithViewModelFilter(Predicate<Type> filter)
	{
		_viewModelFilters.Add(filter);
		return this;
	}
}

/// <summary>
/// Matches values by names
/// </summary>
public class NamespaceConventionPattern : IMvvmMappingPattern
{
	/// <summary>
	/// Wildcard to match the start of namespace mapping e.g. "ViewModels"
	/// </summary>
	public Regex ViewModelPattern { get; set; } = new Regex(@".+\.ViewModels\.(?<match>.+)", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));
	
	/// <summary>
	/// Removes the end parts from a full name
	/// </summary>
	public Regex ViewModelTruncateEndPattern { get; set; } = new Regex(@"(?:ViewModel|Model|VM)$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));
	
	/// <summary>
	/// Wildcard to match the start of namespace mapping e.g. "Views"
	/// </summary>
	public Regex ViewPattern { get; set; } = new Regex(@".+\.Views\.(?<match>.+)", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));
	
	/// <summary>
	/// Removes the end parts from a full name
	/// </summary>
	public Regex ViewTruncateEndPattern { get; set; } = new Regex(@"(?:Page|View)$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="input"></param>
	/// <returns></returns>
	/// <exception cref="NotImplementedException"></exception>
	public MvvmMappingResult GetResult(MvvmMappingInput input)
	{
		var viewTypes = input.ViewTypes.Value;
		var modelTypes = input.ViewModelTypes.Value;

		string? GetMatchingName(string fullName, Regex matchPattern, Regex truncateEndPattern)
		{
			if (!matchPattern.IsMatch(fullName))
				return null;
			
			var match = matchPattern.Match(fullName);
			var matchText = match.Groups["match"].Success ? match.Groups["match"].Value : null;
			if (matchText == null)
				return null;
			
			return truncateEndPattern.Replace(matchText, string.Empty);
		}

		var viewMap = viewTypes
			.Select(d => (type: d, name: GetMatchingName(d.FullName, ViewPattern, ViewTruncateEndPattern)))
			.Where(d => d.name != null)
			.ToDictionary(d => d.name!, d => d.type);
		var viewModelMap = modelTypes
			.Select(d => (type: d, name: GetMatchingName(d.FullName, ViewModelPattern, ViewModelTruncateEndPattern)))
			.Where(d => d.name != null)
			.ToDictionary(d => d.name!, d => d.type);

		var mapped = new HashSet<(Type view, Type viewModel)>();
		var mappedView = new HashSet<Type>();
		var mappedViewModel = new HashSet<Type>();
		foreach (var viewType in viewMap)
		{
			if (viewModelMap.TryGetValue(viewType.Key, out var viewModel))
			{
				mapped.Add((viewType.Value, viewModel));
				mappedView.Add(viewType.Value);
				mappedViewModel.Add(viewModel);
			}
		}
		
		var missingViews = viewMap
			.Select(d => d.Value)
			.Where(d => !mappedView.Contains(d))
			.ToArray();
		var missingViewModels = viewModelMap
			.Select(d => d.Value)
			.Where(d => !mappedViewModel.Contains(d))
			.ToArray();
		var results = mapped
			.Select(d => (d.viewModel, d.view))
			.ToArray();
		
		return new MvvmMappingResult(results, missingViews, missingViewModels);
	}
}

internal interface IMvvmMappingPattern
{
	MvvmMappingResult GetResult(MvvmMappingInput input);
}

/// <summary>
/// Results of the mapping process
/// </summary>
public class MvvmMappingResult
{
	/// <summary>
	/// </summary>
	/// <param name="matches"></param>
	/// <param name="mismatchedViews"></param>
	/// <param name="mismatchedViewModels"></param>
	public MvvmMappingResult((Type viewModelType, Type viewType)[] matches, Type[] mismatchedViews, Type[] mismatchedViewModels)
	{
		Matches = matches;
		MismatchedViews = mismatchedViews;
		MismatchedViewModels = mismatchedViewModels;
	}

	/// <summary>
	/// Matches built from the input
	/// </summary>
	public (Type viewModelType, Type viewType)[] Matches { get; private set; }
	
	/// <summary>
	/// Mismatched entries from the view types
	/// </summary>
	public Type[] MismatchedViews { get; private set; }
	
	/// <summary>
	/// Mismatched entries from the viewmodel types
	/// </summary>
	public Type[] MismatchedViewModels { get; private set; }
}