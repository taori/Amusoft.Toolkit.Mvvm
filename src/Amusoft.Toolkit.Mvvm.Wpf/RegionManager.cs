using System.Windows;
using System.Windows.Controls;
using Amusoft.Toolkit.Mvvm.Core;

namespace Amusoft.Toolkit.Mvvm.Wpf;

/// <summary>
/// RegionManager for WPF platform which integrates with <see cref="INavigationService"/>
/// </summary>
public class RegionManager
{
	/// <summary>
	/// </summary>
	public static readonly DependencyProperty RegionNameProperty = DependencyProperty.RegisterAttached(
		"RegionName",
		typeof(string),
		typeof(RegionManager),
		new PropertyMetadata(null, RegionNameChanged)
	);

	private static void RegionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
	{
		if(e.NewValue is not string regionName)
			throw new RegionManagerException("RegionName must be a string of length > 0");
		
		RegionRegister.RegisterRegion(new ContentControlRegionControl(d as ContentControl, regionName));
	}

	/// <summary>
	/// Sets the region name
	/// </summary>
	/// <param name="element"></param>
	/// <param name="value"></param>
	[AttachedPropertyBrowsableForType(typeof(ContentControl))]
	public static void SetRegionName(ContentControl element, string value)
	{
		element.SetValue(RegionNameProperty, value);
	}

	/// <summary>
	/// Gets the region name
	/// </summary>
	/// <param name="element"></param>
	/// <returns></returns>
	[AttachedPropertyBrowsableForType(typeof(ContentControl))]
	public static string GetRegionName(ContentControl element)
	{
		return (string)element.GetValue(RegionNameProperty);
	}
}