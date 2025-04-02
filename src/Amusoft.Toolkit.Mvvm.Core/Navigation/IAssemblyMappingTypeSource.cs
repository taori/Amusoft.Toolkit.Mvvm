using System;
using System.Reflection;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Provides types for mapping operations in a builder pattern that depends on assemblies
/// </summary>
public interface IAssemblyMappingTypeSource : IMappingTypeSource
{
	/// <summary>
	/// Picks assemblies for the mapping process
	/// </summary>
	/// <param name="assembly"></param>
	/// <returns></returns>
	IAssemblyMappingTypeSource WithAssembly(Assembly assembly);

	/// <summary>
	/// Uses the given filter to narrow down view types
	/// </summary>
	/// <param name="filter"></param>
	/// <returns></returns>
	IAssemblyMappingTypeSource WithViewFilter(Predicate<Type> filter);

	/// <summary>
	/// Uses the given filter to narrow down view types
	/// </summary>
	/// <param name="filter"></param>
	/// <returns></returns>
	IAssemblyMappingTypeSource WithViewModelFilter(Predicate<Type> filter);
}