// This file is licensed to you under the MIT license.

using System.Diagnostics.CodeAnalysis;

namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IRegionRegister
{
	void RegisterRegion(IRegionControl? control);
	bool TryGetRegion(string name, [NotNullWhen(true)] out IRegionControl? control);
}