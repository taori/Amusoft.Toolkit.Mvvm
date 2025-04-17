namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IRegionModelStore
{
	object? GetLastModel(string regionName);
	void PushModel(string regionName, object model);
	void Clear(string regionName);
	bool HasEntries(string regionName);
}