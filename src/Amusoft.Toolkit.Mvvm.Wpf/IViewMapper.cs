// This file is licensed to you under the MIT license.

using System.Windows;

namespace Amusoft.Toolkit.Mvvm.Wpf;

/// <summary>
/// Maps views to a given target
/// </summary>
public interface IViewMapper
{
	/// <summary>
	/// Maps views to the resource dictionary
	/// </summary>
	/// <param name="dictionary">This should be the Resource dictionary of the application</param>
	void Map(ResourceDictionary dictionary);
}