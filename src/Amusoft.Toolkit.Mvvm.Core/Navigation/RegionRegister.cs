using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Amusoft.Toolkit.Mvvm.Core;

internal static class RegionRegister
{
	private static readonly Dictionary<string, IRegionControl> RegionByName = new();

	public static void RegisterRegion(IRegionControl? control)
	{
		if(control is null)
			throw new ArgumentNullException(nameof(control));
		
		RegionByName[control.RegionName] = control;
	}

	public static bool TryGetRegion(string name, [NotNullWhen(true)] out IRegionControl? control)
	{
		control = null;
		if (!RegionByName.TryGetValue(name, out control))
			return false;

		return true;
	}
}