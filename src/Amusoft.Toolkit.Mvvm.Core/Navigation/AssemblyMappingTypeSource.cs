using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class AssemblyMappingTypeSource
	: IAssemblyMappingTypeSource
{
	internal AssemblyMappingTypeSource()
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
	public IAssemblyMappingTypeSource WithAssembly(Assembly assembly)
	{
		_assemblies.Add(assembly);
		return this;
	}

	/// <summary>
	/// Uses the given filter to narrow down view types
	/// </summary>
	/// <param name="filter"></param>
	/// <returns></returns>
	public IAssemblyMappingTypeSource WithViewFilter(Predicate<Type> filter)
	{
		_viewFilters.Add(filter);
		return this;
	}

	/// <summary>
	/// Uses the given filter to narrow down view types
	/// </summary>
	/// <param name="filter"></param>
	/// <returns></returns>
	public IAssemblyMappingTypeSource WithViewModelFilter(Predicate<Type> filter)
	{
		_viewModelFilters.Add(filter);
		return this;
	}

	Type[] IMappingTypeSource.GetViewTypes() => ViewTypes.Value;

	Type[] IMappingTypeSource.GetViewModelTypes() => ViewModelTypes.Value;
}