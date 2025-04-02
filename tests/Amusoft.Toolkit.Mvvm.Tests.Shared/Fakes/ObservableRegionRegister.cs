using System.Diagnostics.CodeAnalysis;

using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

internal class ObservableRegionRegister : IRegionRegister
{
	public Dictionary<string, IRegionControl> ControlsByName = new();

	public void RegisterRegion(IRegionControl? control)
	{
		if (control is null)
			return;

		ControlsByName.Add(control.RegionName, control);
	}

	public bool TryGetRegion(string name, [NotNullWhen(true)] out IRegionControl? control)
	{
		control = default;
		if (!ControlsByName.TryGetValue(name, out var value))
			return false;

		control = value;
		return control != null!;
	}
}