using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Tests.Shared.Fakes;

internal class FakeRegionControl : IRegionControl
{
	private readonly WeakReference<object?> _content = new(null);
	public FakeRegionControl(string regionName, object? content)
	{
		RegionName = regionName;
		Content = content;
	}

	public string RegionName { get; }
	
	public object? Content
	{
		get => _content.TryGetTarget(out var f) ? f : null;
		set => _content.SetTarget(value);
	}
}