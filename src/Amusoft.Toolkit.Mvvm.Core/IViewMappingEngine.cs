// This file is licensed to you under the MIT license.

using System;
using System.Collections.Generic;

namespace Amusoft.Toolkit.Mvvm.Core;

/// <summary>
/// Provides an array of types that are mapped together as datatemplates
/// </summary>
public interface IViewMappingEngine
{
	/// <summary>
	/// Gets an array for templates that have to be mapped
	/// </summary>
	/// <returns></returns>
	HashSet<(Type view, Type viewModel)> GetMappings();
}