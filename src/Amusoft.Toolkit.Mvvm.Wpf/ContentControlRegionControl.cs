using System;
using System.Windows.Controls;
using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Wpf;

internal class ContentControlRegionControl : IRegionControl
{
	public string RegionName { get; }
	
	private readonly WeakReference<ContentControl> _control;

	public ContentControlRegionControl(ContentControl? control, string regionName)
	{
		RegionName = regionName ?? throw new ArgumentNullException(nameof(regionName));
		if(control is null)
			throw new ArgumentNullException(nameof(control));
		_control = new WeakReference<ContentControl>(control);
	}
	
	public object? Content
	{
		get => _control.TryGetTarget(out var c) 
			? c.Content 
			: null;
		set
		{
			if (!_control.TryGetTarget(out var c))
				return;
			c.Content = value;
		}
	}
}