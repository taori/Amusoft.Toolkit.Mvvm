using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Amusoft.Toolkit.Mvvm.Core;

internal class RegionRegister : IRegionRegister
{
	private readonly Dictionary<string, IRegionControl> RegionByName = new();

	internal static readonly RegionRegister Instance = new();

	private RegionRegister(){}

	public void RegisterRegion(IRegionControl? control)
	{
		if(control is null)
			throw new ArgumentNullException(nameof(control));
		
		RegionByName[control.RegionName] = control;
	}

	public bool TryGetRegion(string name, [NotNullWhen(true)] out IRegionControl? control)
	{
		control = null;
		if (!RegionByName.TryGetValue(name, out control))
			return false;

		return true;
	}
}