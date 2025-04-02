using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Matches values by names
/// </summary>
public class NamespaceConventionViewModelToViewMapper : IViewModelToViewMapper
{
	/// <summary>
	/// Wildcard to match the start of namespace mapping e.g. "ViewModels"
	/// <para>The default is: .+\.ViewModels\.(?&lt;match>.+)</para>
	/// </summary>
	public Regex ViewModelPattern { get; set; } = new(@".+\.ViewModels\.(?<match>.+)", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

	/// <summary>
	/// Removes the end parts from a full name
	/// <para>The default is: (?:ViewModel|Model|VM)</para>
	/// </summary>
	public Regex ViewModelTruncateEndPattern { get; set; } = new(@"(?:ViewModel|Model|VM)$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

	/// <summary>
	/// <para>Wildcard to match the start of namespace mapping e.g. "Views"</para>
	/// <para>The default is: .+\.Views\.(?&lt;match>.+)</para>
	/// </summary>
	public Regex ViewPattern { get; set; } = new(@".+\.Views\.(?<match>.+)", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

	/// <summary>
	/// Removes the end parts from a full name
	/// </summary>
	public Regex ViewTruncateEndPattern { get; set; } = new(@"(?:Page|View)$", RegexOptions.Compiled | RegexOptions.Singleline, TimeSpan.FromSeconds(1));

	/// <summary>
	/// 
	/// </summary>
	/// <param name="mappingTypeSource"></param>
	/// <returns></returns>
	public MvvmMappingResult GetResult(IMappingTypeSource mappingTypeSource)
	{
		var viewTypes = mappingTypeSource.GetViewTypes();
		var modelTypes = mappingTypeSource.GetViewModelTypes();

		string? GetMatchingName(string fullName, Regex matchPattern, Regex truncateEndPattern)
		{
			if (!matchPattern.IsMatch(fullName))
				return null;

			var match = matchPattern.Match(fullName);
			var matchText = match.Groups["match"].Success
				? match.Groups["match"].Value
				: null;

			if (matchText == null)
				throw new MvvmCoreException("The regex group \"match\" is missing.");

			return truncateEndPattern.Replace(matchText, string.Empty);
		}

		var viewMap = viewTypes
			.Select(d => (type: d, name: GetMatchingName(d.FullName!, ViewPattern, ViewTruncateEndPattern)))
			.Where(d => d.name != null)
			.ToDictionary(d => d.name!, d => d.type);
		var viewModelMap = modelTypes
			.Select(d => (type: d, name: GetMatchingName(d.FullName!, ViewModelPattern, ViewModelTruncateEndPattern)))
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