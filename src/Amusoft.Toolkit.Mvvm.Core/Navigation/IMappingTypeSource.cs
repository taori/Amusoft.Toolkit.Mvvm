using System;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Source for mapping operations
/// </summary>
public interface IMappingTypeSource
{
	/// <summary>
	/// See method name
	/// </summary>
	/// <returns></returns>
	Type[] GetViewTypes();
	
	/// <summary>
	/// See method name
	/// </summary>
	/// <returns></returns>
	Type[] GetViewModelTypes();
}