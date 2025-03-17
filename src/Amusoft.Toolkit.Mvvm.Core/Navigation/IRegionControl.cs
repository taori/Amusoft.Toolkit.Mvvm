namespace Amusoft.Toolkit.Mvvm.Core;

internal interface IRegionControl
{
	string RegionName { get; }
	object? Content { get; set; }
}