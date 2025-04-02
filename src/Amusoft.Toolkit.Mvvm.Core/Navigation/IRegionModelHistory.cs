// This file is licensed to you under the MIT license.

namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IRegionModelHistory
{
	object? GetLastRegionModel(string regionName);
	void PushEntry(string regionName, object model);
	void Clear(string regionName);
	bool HasHistory(string regionName);
}